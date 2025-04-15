using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace SampleProject.Shared.Models.Misc
{
    public class APIResponseModel<T> : APIResponseModel
    {
        public APIResponseModel(T value)
        {
            Value = value;
        }
        public T? Value { get; private set; }

        public void AddValue(T value)
        {
            Value = value;
        }
    }
    public class APIResponseModel
    {
        #region Properties
        public bool IsSuccess { get; set; }

        public List<string> Errors { get; set; } = [];

        public Dictionary<string, string[]> ValidationErrors { get; set; } = [];

        public List<string> Successes { get; set; } = [];
        #endregion


        #region SuccessMessage
        public void AddSuccessMessage(string message)
        {
            if (!string.IsNullOrEmpty(message) && !Successes.Contains(message))
            {
                Successes.Add(message);
            }
        }
        #endregion

        #region ErrorMessage
        public void AddErrorMessage(string message)
        {
            if (!string.IsNullOrEmpty(message) && !Errors.Contains(message))
            {
                Errors.Add(message);
            }
        }
        #endregion

        #region ValidationErrorMessage
        public void AddValidationErrorMessages(Dictionary<string, string[]> errors)
        {
            ValidationErrors = errors;
        }
        #endregion


        #region OK
        public void OK()
        {
            Succeed("success");
        }
        #endregion


        #region Redirect
        public void Redirect()
        {
            Succeed("redirect");
        }
        #endregion

        #region BadRequest
        public void BadRequest(string message, Dictionary<string, string[]> errors)
        {
            Failed(message, errors);
        }
        #endregion

        #region Unauthorized
        public void Unauthorized(string message)
        {
            Failed(message);
        }
        #endregion

        #region Forbidden
        public void Forbidden(string message)
        {
            Failed(message);
        }
        #endregion

        #region NotFound
        public void NotFound(string message)
        {
            Failed(message);
        }
        #endregion

        #region MethodNotAllowed
        public void MethodNotAllowed(string message)
        {
            Failed(message);
        }
        #endregion

        #region Conflict
        public void Conflict(string message)
        {
            Failed(message);
        }
        #endregion

        #region TooManyRequest
        public void TooManyRequest(string message)
        {
            Failed(message);
        }
        #endregion

        #region NotImplemented
        public void NotImplemented(string message)
        {
            Failed(message);
        }
        #endregion

        #region InternalServerError
        public void InternalServerError(string message)
        {
            Failed(message);
        }
        #endregion


        #region IsSuccess
        private void Succeed(string message)
        {
            AddSuccessMessage(message);
            IsSuccess = true;
        }

        private void Failed(string message)
        {
            AddErrorMessage(message);
            IsSuccess = false;
        }

        private void Failed(string message, Dictionary<string, string[]> errors)
        {
            Failed(message);
            AddValidationErrorMessages(errors);
        }
        #endregion
    }
}
