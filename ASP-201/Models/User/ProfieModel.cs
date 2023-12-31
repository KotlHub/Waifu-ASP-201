﻿namespace ASP_201.Models.User
{
    public class ProfieModel
    {
        public Guid     Sid { get; set; }   // id in database
        public string   RealName { get; set; }
        public bool     IsRealNamePublic { get; set; }

        public string   Login { get; set; }

        public string   Email { get; set; }
        public bool     IsEmailPublic { get; set; }
        public bool     IsEmailConfirmed { get; set; }

        public string   Password { get; set; }
        public string   Avatar { get; set; }
        public DateTime RegisterDt { get; set; }
        public bool IsDatetimesPublic { get; set; }

        /// <summary>
        /// Чи є данний профіль персональним (для автентифікованого користувача)
        /// </summary>
        public bool IsPersonal { get; set; }


        public ProfieModel(Data.Entity.User user)
        {
            var thisProps = this.GetType().GetProperties();
            foreach (var prop in user.GetType().GetProperties())
            {
                var thisProp = thisProps.FirstOrDefault(p => p.Name == prop.Name
                && p.PropertyType.IsAssignableFrom(prop.PropertyType));

                thisProp?.SetValue(this, prop.GetValue(user));
            }
            this.IsEmailConfirmed = user.EmailCode is null;
        }

    }
}
