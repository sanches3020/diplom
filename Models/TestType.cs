namespace Sofia.Web.Models
{
    public enum TestType
    {
        BuiltIn = 0,     // встроенный тест, созданный системой
        Custom = 1,      // пользовательский или созданный психологом
        External = 2     // импортированный из внешнего источника
    }
}
