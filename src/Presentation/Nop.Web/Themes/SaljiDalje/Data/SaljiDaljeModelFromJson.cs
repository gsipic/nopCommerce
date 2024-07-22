using Newtonsoft.Json;

namespace Nop.Web.Themes.SaljiDalje.Data;

public class ChildCategory
{
    public string CategoryName { get; set; }
    public string CategoryImage { get; set; }
    [JsonProperty("ChildCategory")]
    public List<ChildCategory> NestedChildCategory { get; set; }
}

public class AutoMoto   
{
    public string CategoryName { get; set; }
    public string CategoryImage { get; set; }
    public List<ChildCategory> ChildCategory { get; set; }
}

public class SaljiDaljeCategory
{
    [JsonProperty("Auto Moto")]
    public AutoMoto AutoMoto { get; set; }
}