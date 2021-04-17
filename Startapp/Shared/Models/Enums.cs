using System.ComponentModel.DataAnnotations;

namespace Startapp.Shared.Models
{
    public enum Gender
    {
        Female,
        Male
    }
    public enum ArticleCategory
    {
        [Display(Name = "Laptops")]
        Laptops = 1,
        [Display(Name = "Smartphones")]
        Smartphones = 2,
        [Display(Name = "Tablets")]
        Tablets = 3,
        [Display(Name = "Headphones")]
        Headphones = 4,
        [Display(Name = "Televisions")]
        TV = 5,
        [Display(Name = "Cameras")]
        Cameras = 6,
        [Display(Name = "Accesoires")]
        Accesoires = 7       
    }

    public enum Duration
    {
        [Display(Name = "شهر")]
        OneMonths = 1,
        [Display(Name = "شهرين")]
        TwoMonths = 2,
        [Display(Name = "ثلاثة اشهر")]
        ThreeMonths = 3,
        [Display(Name = "اربعة اشهر")]
        FourMonths = 4,
        [Display(Name = "5 اشهر")]
        FiveMonths = 5,
        [Display(Name = "ستة اشهر")]
        SixMonths = 6,
        [Display(Name = "سبعة اشهر")]
        SevenMonths = 7,
        [Display(Name = "ثمانية اشهر")]
        EightMonths = 8,
        [Display(Name = "تسعة اشهر")]
        NineMonths = 9,
        [Display(Name = "10 اشهر")]
        TenMonths = 10,
        [Display(Name = "11 اشهر")]
        ElevenMonths = 11,
        [Display(Name = "12 اشهر")]
        TwelveMonths = 12,
    }

    public enum ArticleLanguage
    {
        [Display(Name = "Arabic - الـعـربـيـة")]
        Arabic = 1,
        [Display(Name = "English - US English (Default)")]
        English = 2,
        [Display(Name = "Chinese - 中文")]
        Chinese = 3,
        [Display(Name = "Indian - हिन्दी")]
        Hindi = 4,
        [Display(Name = "Spanish - Español")]
        Spanish = 5,
        [Display(Name = "Frensh - Français")]
        French = 6,
        [Display(Name = "Bengali - বাংলা")]
        Bengali = 7,
        [Display(Name = "Russian - Русский")]
        Russian = 8,
        [Display(Name = "Portuguese - Português")]
        Portuguese = 9,
        [Display(Name = "Indonisan - Bahasa Indonesia")]
        Indonesian = 10,
        [Display(Name = "German - Deutsch")]
        German = 11,
        [Display(Name = "Japanese - 日本語")]
        Japanese = 12,
        [Display(Name = "Corian - 한국어")]
        Corian = 13,
        [Display(Name = "Turkish - Türkçe")]
        Turkish = 14,
        [Display(Name = "Italian - Italiano")]
        Italian = 15,
    }
    public enum Negotiable
    {
        [Display(Name = "قابل للتفاوض")]
        Negotiable = 1,
        [Display(Name = "غير قابل للتفاوض")]
        NonNegotiable = 2,
        [Display(Name = "معروض")]
        Offert = 3
    }

    public enum SettingsLanguage
    {
        [Display(Name = "Arabic - الـعـربـيـة")]
        Arabic = 1,       
        [Display(Name = "English - US English (Default)")]
        English = 2,
        [Display(Name = "Chinese - 中文")]
        Chinese = 3,
        [Display(Name = "Indian - हिन्दी")]
        Hindi = 4,
        [Display(Name = "Spanish - Español")]
        Spanish = 5,
        [Display(Name = "Frensh - Français")]
        French = 6,
        [Display(Name = "Bengali - বাংলা")]
        Bengali = 7,
        [Display(Name = "Russian - Русский")]
        Russian = 8,
        [Display(Name = "Portuguese - Português")]
        Portuguese = 9,
        [Display(Name = "Indonisan - Bahasa Indonesia")]
        Indonesian = 10,
        [Display(Name = "German - Deutsch")]
        German = 11,
        [Display(Name = "Japanese - 日本語")]
        Japanese = 12,
        [Display(Name = "Corian - 한국어")]
        Corian = 13,
        [Display(Name = "Turkish - Türkçe")]
        Turkish = 14,
        [Display(Name = "Italian - Italiano")]
        Italian = 15,
    }
    public enum Country
    {
        [Display(Name = "Algeria")]
        Algeria = 1,
        [Display(Name = "United Stat of America")]
        USA = 2,
        [Display(Name = "China")]
        China = 3,
        [Display(Name = "India")]
        India = 4,
        [Display(Name = "Spain")]
        Spain = 5,
        [Display(Name = "France")]
        France = 6,
        [Display(Name = "Bangladesh")]
        Bangladesh = 7,
        [Display(Name = "Russia")]
        Russia = 8,
        [Display(Name = "Portugal")]
        Portugal = 9,
        [Display(Name = "Indonesia")]
        Indonesia = 10,
        [Display(Name = "Germany")]
        Germany = 11,
        [Display(Name = "Japan")]
        Japan = 12,
        [Display(Name = "Korea")]
        Korea = 13,
        [Display(Name = "Turkey")]
        Turkey = 14,
        [Display(Name = "Italy")]
        Italia = 15,
    }
}
