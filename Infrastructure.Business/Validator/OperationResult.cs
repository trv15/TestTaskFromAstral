using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Business.Validator
{
    public class OperationResult
    {
        public bool Succeeded { get; private set; }//результат ввиде bool
        public IEnumerable<ValidationException> Errors { get; private set; }

        private static OperationResult _result = new OperationResult(true);

        /// <summary>
        ///Данный конструктор если нет ошибок(По умолчанию сразу установлено в true - _result)
        /// </summary>
        /// <param name="success"></param>
        protected OperationResult(bool success)
        {
            if (Errors == null)
                Errors = new List<ValidationException>();

            this.Succeeded = success;
        }

        /// <summary>
        ///Данное свойство если операция прошла успешно то получаем по умолчанию - new OperationResult(true)
        /// </summary>
        public static OperationResult GetSuccess
        {
            get
            {
                return OperationResult._result;
            }
        }

        /// <summary>
        /// Свойство если операция прошла не успешно
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static OperationResult GetFailed(List<ValidationException> errors)
        {
            return new OperationResult(errors);
        }

        /// <summary>
        /// Конструктор если есть ошибки 
        /// </summary>
        /// <param name="errors"></param>
        public OperationResult(IEnumerable<ValidationException> errors)
        {
            if (errors == null)
                errors = new List<ValidationException>();

            this.Succeeded = false;
            this.Errors = errors;
        }
    }
}
