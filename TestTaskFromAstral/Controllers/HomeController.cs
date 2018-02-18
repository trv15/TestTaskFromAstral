using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using TestTaskFromAstral.Models;
using Infrastructure.Data.Interfaces;
using Infrastructure.Data.UnitOfWork;
using Infrastructure.Data.Context;
using System.Data.Entity;
using Domain.Core;
using Infrastructure.Business.Manager;
using Infrastructure.Business.Validator;
using Infrastructure.Business.Interfaces;

namespace TestTaskFromAstral.Controllers
{
    public class HomeController : Controller
    {
        UnitOfWork work;

        VacancyManager _vakManager;
        TypeVakancyManager _typeManager;
        EmploymentManager _emplManager;
        SalaryManager _salManager;
        AddressManager _addManager;
        ContactsManager _contManager;
        PhonesManager _phonManager;

        public HomeController(IUnitOfWork work)
        {
            this.work = (UnitOfWork)work;

            _vakManager = new VacancyManager(work.getApplicationContext);
            _typeManager = new TypeVakancyManager(work.getApplicationContext);
            _emplManager = new EmploymentManager(work.getApplicationContext);
            _salManager = new SalaryManager(work.getApplicationContext);
            _addManager = new AddressManager(work.getApplicationContext);
            _contManager = new ContactsManager(work.getApplicationContext);
            _phonManager = new PhonesManager(work.getApplicationContext);
        }

        //стартовая
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Index(string vacancyName, int pageNumber = 0)
        {
            ViewBag.VacancyName = vacancyName;

            MainObjectViewModel mainObject = null;
            try
            {
                string jsonResponseText = await GetJsonResponseTextToUrl("https://api.hh.ru/vacancies", vacancyName, pageNumber, 10);//строка в JSON

                mainObject = await GetMainObjectFromJsonObjest(null, jsonResponseText);

                await saveDataToDB(mainObject);

                return View("Index", mainObject);
            }
            catch(Exception ex)
            {
                AddErrors(ex.Message);
                mainObject = await ReadDataFromDB(vacancyName, pageNumber, 10);
                return View("Index", mainObject);
            }
        }
        //считываем из БД данные по вакансиям
        private async Task<MainObjectViewModel> ReadDataFromDB(string vacancyName,int pageNumber = 0, int pageSize = 20)
        {
            MainObjectViewModel  mainObject = new MainObjectViewModel();

            IEnumerable<Vacancy> vacancyList = null;

            if (String.IsNullOrEmpty(vacancyName))
            {
                //количество стр
                mainObject.Pages = (_vakManager.QueryableSet.ToList().Count() / pageSize).ToString();
                //всего найдено
                mainObject.Found = _vakManager.QueryableSet.ToList().Count().ToString();

                vacancyList = await _vakManager.QueryableSet.OrderByDescending(x=>x.Id).Skip(pageNumber*pageSize).Take(pageSize).ToListAsync();
            }
            else
            {
                vacancyName = vacancyName.Trim();
                //всего найдено
                mainObject.Found = _vakManager.QueryableSet.Where(n => n.Name.ToUpper().Equals(vacancyName.ToUpper())).ToList().Count().ToString();
                //количество стр
                mainObject.Pages = (_vakManager.QueryableSet.Where(n => n.Name.ToUpper().Equals(vacancyName.ToUpper())).ToList().Count() / pageSize).ToString();
                //список вакансии
                vacancyList = _vakManager.QueryableSet.Where(n => n.Name.ToUpper().Equals(vacancyName.ToUpper()))
                  .OrderByDescending(x => x.Id).Skip(pageNumber * pageSize).Take(pageSize).ToList();

            }

            //текущая стр
            mainObject.Page = pageNumber.ToString();
            //на стр
            mainObject.Per_Page = pageSize.ToString();
            //коллекция вакансии
            mainObject.Items = new List<VacancyViewModel>();

            foreach (Vacancy vacancy in vacancyList)
            {
                VacancyViewModel viewItem = new VacancyViewModel()
                {
                    Id = vacancy.Id,
                    Name = vacancy.Name,
                    Description = vacancy.Description,
                    Archived = vacancy.Archived,
                    Published_At = vacancy.Published_At,
                    Type = new TypeVacancyViewModel()
                    {
                        Id = vacancy.TypeVakancyId,
                        Name = vacancy.TypeVakancy.Name
                    }
                };
                if (vacancy.Salary != null)
                {
                    viewItem.Salary = new SalaryViewModel()
                    {
                        Currency = vacancy.Salary.Currency,
                        From = vacancy.Salary.From,
                        To = vacancy.Salary.To,
                        Gross = vacancy.Salary.Gross
                    };
                }
                if (vacancy.Address != null)
                {
                    viewItem.Address = new AddressViewModel()
                    {
                        City = vacancy.Address.City,
                        Street = vacancy.Address.Street,
                        Building = vacancy.Address.Building,
                        Description = vacancy.Address.Description
                    };
                }
                if (vacancy.Employment != null)
                {
                    viewItem.Employment = new EmploymentViewModel()
                    {
                        Id = vacancy.EmploymentId,
                        Name = vacancy.Employment.Name
                    };
                }

                mainObject.Items.Add(viewItem);
            }

            return mainObject;
    }
        //сохраняем или обновляем в бд данные о вакансии(неполные данные)
        private async Task saveDataToDB(MainObjectViewModel mainObject)
        {
            foreach(VacancyViewModel item in mainObject.Items)
            {
                //тип вакансии(открытая закрытая)
                if (item.Type != null)
                {
                    OperationResult resultType = await _typeManager.CreateAsync(new TypeVacancy()
                    {
                        Id = item.Type.Id,
                        Name = item.Type.Name
                    });
                }
                //вид вакансии(полный раб по совместительству)
                if (item.Employment != null)
                {
                    OperationResult resultEmpl = await _emplManager.CreateAsync(new Employment()
                    {
                        Id = item.Employment.Id,
                        Name = item.Employment.Name
                    });
                }
                //вакансии
                OperationResult resultVak = await _vakManager.CreateAsync(new Vacancy()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Published_At = item.Published_At,
                    Description = item.Description,
                    Archived = item.Archived,
                    TypeVakancyId = item.Type != null ? item.Type.Id : null,
                    EmploymentId = item.Employment != null ? item.Employment.Id : null
                });
                //адреса 
                if (item.Address != null)
                {
                    OperationResult resultAddr = await _addManager.CreateAsync(new Address()
                    {
                        Id = item.Id,
                        City = item.Address.City,
                        Street = item.Address.Street,
                        Building = item.Address.Building,
                        Description = item.Address.Description
                    });
                }
                //зарплата
                if (item.Salary != null)
                {
                    OperationResult resultSal = await _salManager.CreateAsync(new Salary()
                    {
                        Id = item.Id,
                        From = item.Salary.From,
                        To = item.Salary.To,
                        Gross =item.Salary.Gross,
                        Currency = item.Salary.Currency
                    });
                }
                //Контаткы
                if (item.Contacts != null)
                {
                    OperationResult resultCon = await _contManager.CreateAsync(new Contacts()
                    {
                        Id = item.Id,
                        Name = item.Contacts.Name,
                        Email = item.Contacts.Email
                    });
                }

                await work.SaveAsync();
            }

            
        }
        //метод десериализация из JSON в пользовательский тип .Net
        [NonAction]
        private Task<MainObjectViewModel> GetMainObjectFromJsonObjest(MainObjectViewModel mainObject, string jsonResponseText)
        {
            if (String.IsNullOrEmpty(jsonResponseText))
                throw new ArgumentNullException("Значение ответа удаленного сервера не может быть пустым");

            return Task<MainObjectViewModel>.Factory.StartNew(() => {
        
                if (mainObject == null)
                    mainObject = new MainObjectViewModel();
                try
                {
                    mainObject = JsonConvert.DeserializeObject<MainObjectViewModel>(jsonResponseText);
                    return mainObject;
                }
                catch(Exception ex)
                {
                    AddErrors("Ошибка десериализации данных формата Json в пользовательский, " + ex.Message);
                    return mainObject;
                }
            });
        }
        //метод отправки основного http запроса и получения ответа
        [NonAction]
        private Task<string> GetJsonResponseTextToUrl(string url, string vacancyName = "", int pageNumber = 0, int pageSize = 20)
        {
            string jsonResponseText = "";

            if (String.IsNullOrEmpty(url))
                throw new ArgumentNullException("Значение url не может быть пустым");

            return Task<string>.Run(() => {

                try
                {
                    //если пользователь ввел лишние пробелы заменяем на 1 пробел 
                    if (!String.IsNullOrEmpty(vacancyName))
                    {
                        vacancyName = Regex.Replace(vacancyName, @"\s+", @" ");
                    }

                    //строка url с поиском по названию вакансии
                    string stringUrl = String.Format(url + "?text={0}&search_field=name&page={1}&per_page={2}", vacancyName, pageNumber, pageSize);

                    //если поиском по названию вакансии не воспользовались
                    if (String.IsNullOrEmpty(vacancyName))
                    {
                        stringUrl = String.Format(url + "?page={0}&per_page={1}", pageNumber, pageSize);
                    }

                    jsonResponseText = HttpRequestUrl(stringUrl, jsonResponseText);
                }
                catch (WebException ex)
                {
                    AddErrors("Скорее всего отсутствует интернет соединение...");
                    AddErrors("Подробное описание ошибки: " + ex.Message);
                }
                catch (Exception ex)
                {
                    AddErrors(ex.Message);
                }

                return jsonResponseText;
            });
        }

        //просмотр вакансии vacancy_id
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> CardVacancyView(string vacancyId)
        {
            VacancyViewModel vacancy = null;
            try
            {
                string jsonResponseText = await GetJsonResponseTextToVaсancyId("https://api.hh.ru/vacancies/", vacancyId);//строка в JSON

                 vacancy = await GetVaсancyFromJsonObjest(null, jsonResponseText);

                if (vacancy == null)
                {
                    AddErrors(String.Format("Не удалось найти вакансию по данномоу ID: {0}", vacancyId));
                    return RedirectToLocal(Request.UrlReferrer.ToString());
                }

                await saveDataToDB(vacancy);

                return View("CardVacancyView", vacancy);
            }
            catch (Exception ex)
            {
                AddErrors(ex.Message);
                vacancy = await ReadDataVacancyFromDB(vacancyId);
                return View("CardVacancyView", vacancy);
            }
        }
        //считываем из БД данные по вакансии
        private async Task<VacancyViewModel> ReadDataVacancyFromDB(string vacancyId)
        {

            if (String.IsNullOrEmpty(vacancyId))
            {
                return null;
            }

            Vacancy vacancy = await _vakManager.FindByIdAsync(vacancyId);

            VacancyViewModel viewItem = new VacancyViewModel()
            {
                Id = vacancy.Id,
                Name = vacancy.Name,
                Description = vacancy.Description,
                Archived = vacancy.Archived,
                Published_At = vacancy.Published_At,
                Type = new TypeVacancyViewModel()
                {
                    Id = vacancy.TypeVakancyId,
                    Name = vacancy.TypeVakancy.Name
                }
            };

            if (vacancy.Salary != null)
            {
                viewItem.Salary = new SalaryViewModel()
                {
                    Currency = vacancy.Salary.Currency,
                    From = vacancy.Salary.From,
                    To = vacancy.Salary.To,
                    Gross = vacancy.Salary.Gross
                };
            }
            if (vacancy.Address != null)
            {
                viewItem.Address = new AddressViewModel()
                {
                    City = vacancy.Address.City,
                    Street = vacancy.Address.Street,
                    Building = vacancy.Address.Building,
                    Description = vacancy.Address.Description
                };
            }
            if (vacancy.Employment != null)
            {
                viewItem.Employment = new EmploymentViewModel()
                {
                    Id = vacancy.EmploymentId,
                    Name = vacancy.Employment.Name
                };
            }
            if (vacancy.Contacts != null)
            {
                viewItem.Contacts = new ContactsViewModel()
                {
                    Name = vacancy.Contacts.Name,
                    Email = vacancy.Contacts.Email
                };

                if(vacancy.Contacts.Phones != null)
                {
                    foreach(Phones phone in vacancy.Contacts.Phones)
                    {
                        viewItem.Contacts.Phones.Add(new PhonesViewModel() {
                            Country = phone.Country,
                            City = phone.City,
                            Number = phone.Number,
                            Comment = phone.Comment
                        });
                    }
                }
            }

            return viewItem;
        }
        //сохраняем или обновляем в БД данные о вакансии
        private async Task saveDataToDB(VacancyViewModel vacancy)
        {
                //тип вакансии(открытая закрытая)
                if (vacancy.Type != null)
                {
                    OperationResult resultType = await _typeManager.CreateAsync(new TypeVacancy()
                    {
                        Id = vacancy.Type.Id,
                        Name = vacancy.Type.Name
                    });
                }
                //вид вакансии(полный раб по совместительству)
                if (vacancy.Employment != null)
                {
                    OperationResult resultEmpl = await _emplManager.CreateAsync(new Employment()
                    {
                        Id = vacancy.Employment.Id,
                        Name = vacancy.Employment.Name
                    });
                }
                //вакансии
                OperationResult resultVak = await _vakManager.CreateAsync(new Vacancy()
                {
                    Id = vacancy.Id,
                    Name = vacancy.Name,
                    Published_At = vacancy.Published_At,
                    Description = vacancy.Description,
                    Archived = vacancy.Archived,
                    TypeVakancyId = vacancy.Type != null ? vacancy.Type.Id : null,
                    EmploymentId = vacancy.Employment != null ? vacancy.Employment.Id : null
                });
                //адреса 
                if (vacancy.Address != null)
                {
                    OperationResult resultAddr = await _addManager.CreateAsync(new Address()
                    {
                        Id = vacancy.Id,
                        City = vacancy.Address.City,
                        Street = vacancy.Address.Street,
                        Building = vacancy.Address.Building,
                        Description = vacancy.Address.Description
                    });
                }
                //зарплата
                if (vacancy.Salary != null)
                {
                    OperationResult resultSal = await _salManager.CreateAsync(new Salary()
                    {
                        Id = vacancy.Id,
                        From = vacancy.Salary.From,
                        To = vacancy.Salary.To,
                        Gross = vacancy.Salary.Gross,
                        Currency = vacancy.Salary.Currency
                    });
                }
                //Контаткы
                if (vacancy.Contacts != null)
                {
                    OperationResult resultCon = await _contManager.CreateAsync(new Contacts()
                    {
                        Id = vacancy.Id,
                        Name = vacancy.Contacts.Name,
                        Email = vacancy.Contacts.Email
                    });

                    //Телефоны
                    if (vacancy.Contacts.Phones != null)
                    {
                        foreach (PhonesViewModel phone in vacancy.Contacts.Phones)
                        {
                            OperationResult resultPhone = await _phonManager.CreateAsync(new Phones()
                            {
                                Country = phone.Country,
                                City = phone.City,
                                Number = phone.Number,
                                Comment = phone.Comment,
                                ContactsId = vacancy.Id

                            });
                        }
                    }
                }
              

                await work.SaveAsync();
        }
        //метод десериализация из JSON в пользовательский тип VacancyViewModel
        [NonAction]
        private async Task<VacancyViewModel> GetVaсancyFromJsonObjest(VacancyViewModel vacancy, string jsonResponseText)
        {
            if (String.IsNullOrEmpty(jsonResponseText))
                throw new ArgumentNullException("Значение ответа удаленного сервера не может быть пустым");

            return await Task<VacancyViewModel>.Factory.StartNew(() => {

                if (vacancy == null)
                    vacancy = new VacancyViewModel();
                try
                {
                    vacancy = JsonConvert.DeserializeObject<VacancyViewModel>(jsonResponseText);
                    return vacancy;
                }
                catch(Exception ex)
                {
                    AddErrors("Ошибка десериализации данных формата Json в пользовательский, " + ex.Message);
                    return vacancy;
                }
            });
        }
        //метод отправки HTTP запроса с vakancyId и получения ответа в формате JSON
        [NonAction]
        private async Task<string> GetJsonResponseTextToVaсancyId(string url, string vacancyId)
        {
            string jsonResponseText = "";

            if (String.IsNullOrEmpty(url))
                throw new ArgumentNullException("Значение url не может быть пустым");
            if (String.IsNullOrEmpty(vacancyId))
                throw new ArgumentNullException("Значение vakancyId не может быть пустым");

            return await Task.Factory.StartNew<string>(() =>
            {

                try
                { 
                    //строка url с поиском по названию вакансии
                    string stringUrl = String.Format(url + vacancyId);

                    jsonResponseText = HttpRequestUrl(stringUrl,jsonResponseText);
                }
                catch (WebException ex)
                {
                    AddErrors("Скорее всего отсутствует интернет соединение...");
                    AddErrors("Подробное описание ошибки: " + ex.Message);
                }
                catch (Exception ex)
                {
                    AddErrors(ex.Message);
                }

                return jsonResponseText;
            });
        }

        private string HttpRequestUrl(string stringUrl, string jsonResponseText)
        {
            Uri uri = new Uri(stringUrl);

            var request = (HttpWebRequest)WebRequest.Create(uri);//запрос
            request.Method = "GET";//тип метода get
            request.Accept = "application/json";//тип заголовка запроса
            request.UserAgent = "HH-User-Agent";
            request.Credentials = CredentialCache.DefaultCredentials;//учетка
            request.Timeout = 10000;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();//ответ

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new WebException("Http статус(код состояния http запроса): " + response.StatusCode.ToString());
            }

            using (Stream dataStream = response.GetResponseStream()) //получаем поток который используется для чтения данных
            {
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    jsonResponseText = reader.ReadToEnd();
                }
            }
            response.Close();

            return jsonResponseText;
        }


        #region HelpMethods
        /*Метод проверки на локальность URL и в случае необходимости перенаправления по заданному локальному URL*/
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        ///*Методы записи ошибок в состояние модели*/
        private void AddErrors(OperationResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Message + " " + error.Property);
            }
        }
        private void AddErrors(string error)
        {
            ModelState.AddModelError("", error);
        }
        private void AddErrors(string[] errors)
        {
            foreach (string error in errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        #endregion

    }
}