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
    [JsonProperty("Osnovni automobili")]
    public AutoMoto AutoMoto { get; set; }
    
    [JsonProperty("Novi automobili")]
    public AutoMoto Novi_automobili { get; set; }
    
    [JsonProperty("Karambolirani automobili")]
    public AutoMoto Karambolirani_automobili { get; set; }
    
    [JsonProperty("Motocikli / Motori")]
    public AutoMoto MotocikliMotori { get; set; }
    
    [JsonProperty("Rezervni dijelovi i oprema")]
    public AutoMoto Rezervni_dijelovi_i_oprema { get; set; }
    
    [JsonProperty("Gospodarska vozila")]
    public AutoMoto Gospodarska_vozila { get; set; }
    
    [JsonProperty("Kamperi i kamp prikolice")]
    public AutoMoto Kamperi_i_kamp_prikolice { get; set; }
    
    [JsonProperty("Oldtimeri")]
    public AutoMoto Oldtimeri { get; set; }
}