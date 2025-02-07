using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FribergCarRentals.Helpers
{

    public static class EnumHelpers
    {

        // This method returns the Name of the Display attribute of a given enum value
        public static string GetDisplayName(this Enum enumValue)
        {
            var enumType = enumValue.GetType();
            var memberInfo = enumType.GetMember(enumValue.ToString());

            // GetMember returns an array of objects. If input value is incorrect the array will have length 0.
            if (memberInfo.Length > 0)
            {
                /*  
                    Specific enum values will always return a memberinfo-array of length 1. 
                    Look for custom attributes that matches DisplayAttribute. Will be null 
                    if there are no custom attributes, otherwise it returns an object that
                    will need to be casted into a DisplayAttribute from which you can access 
                    its custom display name. 
                */
                var attribute = memberInfo[0]
                    .GetCustomAttributes(typeof(DisplayAttribute), false)
                    .FirstOrDefault() as DisplayAttribute;

                if (attribute != null)
                {
                    return attribute.Name;
                }
            }

            return enumValue.ToString();
        }


        /* 
            This method converts any enum to a list of SelectListItem which is used to automatically
            populate a dropdown menu in a view with the values of the input enum.
        */
        public static List<SelectListItem> EnumToDropdown<TEnum>() where TEnum : Enum
        {
            // Enum.GetValues returns an array of objects
            var enumArray = Enum.GetValues(typeof(TEnum));
            var enumSelectList = new List<SelectListItem>();

            // For each enum value, get the value in text form and numeric form
            foreach (var value in enumArray)
            {
                // Return the DisplayName in a string if set, otherwise return value.ToString().
                var displayName = ((Enum)value).GetDisplayName();

                // asp tag helpers expect strings
                var selectListItem = new SelectListItem
                {
                    Text = displayName,
                    Value = Convert.ToInt32(value).ToString()
                };

                // Mark the first item as selected by default
                if (enumSelectList.Count == 0)
                {
                    selectListItem.Selected = true;
                }

                enumSelectList.Add(selectListItem);
            }
            return enumSelectList;
        }
    }
}
