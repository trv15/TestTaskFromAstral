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
using Infrastructure.Data.Validator;

namespace TestTaskFromAstral.Controllers
{
    public class HomeController : Controller
    {
        UnitOfWork work;

        public HomeController(IUnitOfWork work)
        {
            this.work = (UnitOfWork)work;
        }

        //стартовая
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Index(string vakancyName, int pageNumber = 0)
        {
            try
            {
                string jsonResponseText = await GetJsonResponseTextToUrl("https://api.hh.ru/vacancies", vakancyName, pageNumber, 10);//строка в JSON

                MainObjectViewModel mainObject = await GetMainObjectFromJsonObjest(null, jsonResponseText);

                ViewBag.VakancyName = vakancyName;

                return View("Index", mainObject);
            }
            catch(Exception ex)
            {
                AddErrors(ex.Message);
                return View("Index");
            }
        }
             
        //метод десериализация из JSON в пользовательский тип .Net+
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
        //метод отправки основного http запроса и получения ответа+
        [NonAction]
        private Task<string> GetJsonResponseTextToUrl(string url, string vakancyName = "", int pageNumber = 0, int pageSize = 20)
        {
            string jsonResponseText = "";

            if (String.IsNullOrEmpty(url))
                throw new ArgumentNullException("Значение url не может быть пустым");

            return Task<string>.Run(() => {

                try
                {
                    //если пользователь ввел лишние пробелы заменяем на 1 пробел 
                    if (!String.IsNullOrEmpty(vakancyName))
                    {
                        vakancyName = Regex.Replace(vakancyName, @"\s+", @" ");
                    }

                    //строка url с поиском по названию вакансии
                    string stringUrl = String.Format(url + "?text={0}&search_field=name&page={1}&per_page={2}", vakancyName, pageNumber, pageSize);

                    //если поиском по названию вакансии не воспользовались
                    if (String.IsNullOrEmpty(vakancyName))
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
        public async Task<ActionResult> CardVakancyView(string vakancyId)
        {
            try
            {
                string jsonResponseText = await GetJsonResponseTextToVakancyId("https://api.hh.ru/vacancies/", vakancyId);//строка в JSON

                VacancyViewModel vakancy = await GetVakancyFromJsonObjest(null, jsonResponseText);

                if (vakancy == null)
                {
                    AddErrors(String.Format("Не удалось найти вакансию по данномоу ID: {0}", vakancyId));
                    return RedirectToLocal(Request.UrlReferrer.ToString());
                }

                return View("CardVakancyView", vakancy);
            }
            catch (Exception ex)
            {
                AddErrors(ex.Message);
                return View("Index");
            }
        }
        //метод десериализация из JSON в пользовательский тип VacancyViewModel
        [NonAction]
        private async Task<VacancyViewModel> GetVakancyFromJsonObjest(VacancyViewModel vakancy, string jsonResponseText)
        {
            if (String.IsNullOrEmpty(jsonResponseText))
                throw new ArgumentNullException("Значение ответа удаленного сервера не может быть пустым");

            return await Task<VacancyViewModel>.Factory.StartNew(() => {

                if (vakancy == null)
                    vakancy = new VacancyViewModel();
                try
                {
                    vakancy = JsonConvert.DeserializeObject<VacancyViewModel>(jsonResponseText);
                    return vakancy;
                }
                catch(Exception ex)
                {
                    AddErrors("Ошибка десериализации данных формата Json в пользовательский, " + ex.Message);
                    return vakancy;
                }
            });
        }
        //метод отправки HTTP запроса с vakancyId и получения ответа в формате JSON
        [NonAction]
        private async Task<string> GetJsonResponseTextToVakancyId(string url, string vakancyId)
        {
            string jsonResponseText = "";

            if (String.IsNullOrEmpty(url))
                throw new ArgumentNullException("Значение url не может быть пустым");
            if (String.IsNullOrEmpty(vakancyId))
                throw new ArgumentNullException("Значение vakancyId не может быть пустым");

            return await Task.Factory.StartNew<string>(() =>
            {

                try
                { 
                    //строка url с поиском по названию вакансии
                    string stringUrl = String.Format(url + vakancyId);

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