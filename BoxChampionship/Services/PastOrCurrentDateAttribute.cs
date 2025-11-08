using System;
using System.ComponentModel.DataAnnotations;
//Клас для перевірки дати
public class PastOrCurrentDateAttribute : ValidationAttribute
{
    public PastOrCurrentDateAttribute()
    {
        
        ErrorMessage = "Выбранная дата не может быть в будущем.";
    }

    public override bool IsValid(object value)
    {
        
        if (value == null)
        {
            return true; 
        }

        if (value is DateTime)
        {
            DateTime dateToCheck = (DateTime)value;

           
            return dateToCheck <= DateTime.Now;
        }

        
        return false;
    }
}