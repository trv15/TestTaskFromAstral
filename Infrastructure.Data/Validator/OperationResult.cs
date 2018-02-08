using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Validator
{
    public class OperationResult
    {
        public bool Succeeded { get; private set; }//результат ввиде bool
        public IEnumerable<ValidationException> Errors { get; private set; }

        private static OperationResult _result = new OperationResult(true);

        //конструктор если нет ошибок(По умолчанию сразу установлено в true - _result)
        protected OperationResult(bool success)
        {
            if (Errors == null)
                Errors = new List<ValidationException>();

            this.Succeeded = success;
        }

        //свойство если операция прошла успешно то получаем по умолчанию - new OperationResult(true)
        public static OperationResult GetSuccess
        {
            get
            {
                return OperationResult._result;
            }
        }

        //Свойство если операция прошла не успешно
        public static OperationResult GetFailed(List<ValidationException> errors)
        {
            return new OperationResult(errors);
        }

        //конструктор если есть ошибки 
        public OperationResult(IEnumerable<ValidationException> errors)
        {
            if (errors == null)
                errors = new List<ValidationException>();

            this.Succeeded = false;
            this.Errors = errors;
        }
    }
}
