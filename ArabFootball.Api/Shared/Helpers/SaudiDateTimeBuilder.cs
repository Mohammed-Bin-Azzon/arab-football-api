using System.Globalization;

namespace ArabFootball.Shared.Helpers
{
    public static class SaudiDateTimeBuilder
    {
        public static bool TryBuild(
            string matchDate,
            int hour,
            int minute,
            string period,
            out DateTime result,
            out string error)
        {
            result = default;
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(matchDate))
            {
                error = "تاريخ المباراة مطلوب.";
                return false;
            }

            if (!DateTime.TryParseExact(
                    matchDate.Trim(),
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var parsedDate))
            {
                error = "صيغة التاريخ يجب أن تكون yyyy-MM-dd مثل 2026-05-18.";
                return false;
            }

            if (hour < 1 || hour > 12)
            {
                error = "الساعة يجب أن تكون من 1 إلى 12 فقط.";
                return false;
            }

            if (minute < 0 || minute > 59)
            {
                error = "الدقيقة يجب أن تكون من 0 إلى 59 فقط.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(period))
            {
                error = "الفترة مطلوبة: صباح أو مساء.";
                return false;
            }

            var normalizedPeriod = period.Trim().ToLower();

            bool isMorning =
                normalizedPeriod == "صباح" ||
                normalizedPeriod == "صباحا" ||
                normalizedPeriod == "صباحاً" ||
                normalizedPeriod == "am" ||
                normalizedPeriod == "a.m.";

            bool isEvening =
                normalizedPeriod == "مساء" ||
                normalizedPeriod == "مساءا" ||
                normalizedPeriod == "مساءً" ||
                normalizedPeriod == "pm" ||
                normalizedPeriod == "p.m.";

            if (!isMorning && !isEvening)
            {
                error = "الفترة يجب أن تكون صباح أو مساء.";
                return false;
            }

            int hour24 = hour;

            // 12 صباحاً = 00
            if (isMorning && hour == 12)
            {
                hour24 = 0;
            }
            // 1 مساءً إلى 11 مساءً = +12
            else if (isEvening && hour != 12)
            {
                hour24 = hour + 12;
            }

            result = new DateTime(
                parsedDate.Year,
                parsedDate.Month,
                parsedDate.Day,
                hour24,
                minute,
                0,
                DateTimeKind.Unspecified);

            return true;
        }
    }
}