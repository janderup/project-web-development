using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectWebDevelopment.Data
{
    public enum AuctionDuration
    {
        [Description("24 uur")]
        OneDay,

        [Description("48 uur")]
        TwoDays,

        [Description("7 dagen")]
        OneWeek
    }

    public static class AuctionDurationExtensions
    {
        public static TimeSpan ToTimeSpan(this AuctionDuration duration)
        {
            return duration switch
            {
                AuctionDuration.OneDay => TimeSpan.FromHours(24),
                AuctionDuration.TwoDays => TimeSpan.FromHours(48),
                AuctionDuration.OneWeek => TimeSpan.FromDays(7),
                _ => throw new ArgumentException("Invalid auction duration."),
            };
        }


        // see https://medium.com/engineered-publicis-sapient/human-friendly-enums-in-c-9a6c2291111
        static public string GetDescription(this Enum enumValue)
        {
            var field = enumValue.GetType().GetField(enumValue.ToString());
            if (field == null)
                return enumValue.ToString();

            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                return attribute.Description;
            }

            return enumValue.ToString();
        }
    }
}
