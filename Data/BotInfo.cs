namespace OpenAI.Enums
{
    public static class BotInfo
    {
        public enum Gender // enum for gender assignment
        {
            Undefined = 0,
            Male = 1,
            Female = 2
        };

        /// <summary>
        /// Outputs a string to its corresponding BotInfo.Gender enum
        /// </summary>
        /// <param name="gender">Gender enum</param>
        /// <returns>"undefined" / "male" / "female" </returns>
        public static string GenderToString(Gender gender)
        {
            switch (gender) // parse string based on gender enum
            {
                default: // default will always be undefined
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
