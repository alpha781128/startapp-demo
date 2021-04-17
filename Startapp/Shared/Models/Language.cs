namespace Startapp.Shared.Models
{
    public class Language
    {
        public virtual string Culture
        {
            get
            {
                string culture = string.Empty;
                if (!string.IsNullOrWhiteSpace(LanguageCode))
                {
                    culture += LanguageCode;
                }
                if (!string.IsNullOrWhiteSpace(CountryCode))
                {
                    culture += "-" + CountryCode;
                }
                return culture;
            }
        }
        public int Id { get; set; }
        public string LocalName { get; set; }
        public string LanguageCode { get; set; }
        public string CountryCode { get; set; }
        public SettingsLanguage SettingsLanguage { get; set; }
    }
}
