namespace api_training.Shared.Helpers
{
    public class Routing
    {
        public const string Root = "api";
        public const string Rule = Root + "/";
        public static class Courses
        {
            public const string Prefix = Rule + "courses";

            public const string GetAll = Prefix;
            public const string GetById = Prefix + "/{id:int}";
            public const string Details = Prefix + "/details/{id:int}";

            public const string Add = Prefix;
            public const string Update = Prefix + "/{id:int}";
            public const string Delete = Prefix + "/{id:int}";
        }

        public static class Workers
        {
            public const string Prefix = Rule + "workers";

            public const string GetAll = Prefix;
            public const string GetById = Prefix + "/{id:int}";
            public const string Details = Prefix + "/details/{id:int}";

            public const string Add = Prefix;
            public const string Update = Prefix + "/{id:int}";
            public const string Delete = Prefix + "/{id:int}";
        }

        public static class Drivers
        {
            public const string Prefix = Rule + "drivers";
        }
        public static class Students
        {
            public const string Prefix = Rule + "students";

            public const string GetAll = Prefix;
            public const string GetById = Prefix + "/{id:int}";
            public const string Details = Prefix + "/details/{id:int}";

            public const string Add = Prefix;
            public const string Update = Prefix + "/{id:int}";
            public const string Delete = Prefix + "/{id:int}";
        }

        public static class Books
        {
            public const string Prefix = Rule + "books";
            public const string GetAll = Prefix;
            public const string GetById = Prefix + "/{id:int}";
        }
        public static class Products
        {
            public const string Prefix = Rule + "products";

            public const string GetAll = Prefix;//api/products
            public const string GetById = Prefix + "/{id:int}";
            public const string Details = Prefix + "/details/{id:int}";

            public const string Add = Prefix;
            public const string Update = Prefix + "/{id:int}";
            public const string Delete = Prefix + "/{id:int}";
        }
        public static class Majors
        {
            public const string Prefix = Rule + "majors";
            

            public static class Authors
            {
                public const string Prefix = Rule + "Autohrs";

                public const string GetAll = Prefix;
                public const string Add = Prefix;
            }
        }
    }
}
