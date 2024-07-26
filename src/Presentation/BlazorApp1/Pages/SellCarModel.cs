using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Pages;

public class SellCarModel
{
    public BasicInfo BasicInfo { get; set; } = new();
    public Price Price { get; set; } = new();
}

public class BasicInfo
{
    [Required]
    public string Title { get; set; }
    public IList<SpecificationOption> VehicleCondition { get; set; } = new List<SpecificationOption>();
    public string SpecificationAttributeOptionIdStateOptionId { get; set; }
    
    public IList<SpecificationOption> AdType { get; set; } = new List<SpecificationOption>();
    public string SpecificationAttributeAdTypeIdStateOptionId { get; set; }
};

public class SpecificationOption
{
    public string Text { get; set; }
    public string Value { get; set; }
}

public class Price
{ 
    public int SellCarPrice { get; set; }
    public bool NegotiatedPrice { get; set; }
}

public class VehicleInformation
{
    
    
}

public enum AdType
{
    [Description("I am a private seller")]
    Private, 
    [Description("I am a registered dealer")]
    Company
}

public enum VehicleCondition { Used, New }

public static class MyEnumExtensions
{
    public static string ToDescriptionString(this AdType val)
    {
        DescriptionAttribute[] attributes = (DescriptionAttribute[])val
            .GetType()
            .GetField(val.ToString())
            .GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : string.Empty;
    }
} 