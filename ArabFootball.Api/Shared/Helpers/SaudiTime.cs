namespace ArabFootball.Shared.Helpers
{
    public static class SaudiTime
    {
        private static readonly TimeZoneInfo SaudiZone = GetSaudiTimeZone();

        private static TimeZoneInfo GetSaudiTimeZone()
        {
            try
            {
                // Windows
                return TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");
            }
            catch
            {
                // Linux / Docker
                return TimeZoneInfo.FindSystemTimeZoneById("Asia/Riyadh");
            }
        }

        public static DateTime Now()
        {
            var saudiNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, SaudiZone);

            // نخزن الوقت كوقت عادي بدون UTC حتى يكون مفهوم كتوقيت السعودية
            return DateTime.SpecifyKind(saudiNow, DateTimeKind.Unspecified);
        }

        public static DateTime FromUserInput(DateTime dateTime)
        {
            // أي وقت يأتي من Swagger أو الواجهة نعتبره توقيت السعودية كما هو
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
        }
    }
}