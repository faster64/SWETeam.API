using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWETeam.Common.Validations
{
    public class BaseValidator<T> : AbstractValidator<T>
    {
        /// <summary>
        /// Tiến hành validate
        /// </summary>
        public virtual void Execute(T model)
        {
            var result = Validate(model);
            if (!result.IsValid)
            {
                throw new Exception(result.ToString($" {Environment.NewLine} "));
            }
        }
    }
}
