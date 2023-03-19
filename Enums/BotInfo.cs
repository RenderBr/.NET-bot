using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAI.Enums
{
    public static class BotInfo
    {
        public enum Gender
        {
            Undefined = 0,
            Male = 1,
            Female = 2
        };

        public static string GenderToString(Gender gender)
        {
            switch (gender)
            {
                default:
                case Gender.Undefined:
                    return "Undefined";
                case Gender.Male:
                    return "Male";
                case Gender.Female:
                    return "Female";
            }
        }
        
    }
}
