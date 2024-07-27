using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Pages;

public class SellCarModel
{
    public BasicInfo BasicInfo { get; set; }
    public Price Price { get; set; }
    public VehicleInformation VehicleInformation { get; set; }
}

public class BasicInfo
{
    [Required] public string Title { get; set; }
    public IList<SpecificationOption> VehicleCondition { get; set; }
    public string SpecificationAttributeOptionIdStateOptionId { get; set; }

    public IList<SpecificationOption> AdType { get; set; }
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
    private int Year { get; set; } = 0;
    private int Mileage { get; set; } = 0;
    private string VIN { get; set; } = string.Empty;
    public IList<SpecificationOption> BodyType { get; set; } = new List<SpecificationOption>();
    public IList<SpecificationOption> FuelType { get; set; } = new List<SpecificationOption>();
    public IList<SpecificationOption> Engine { get; set; } = new List<SpecificationOption>();
    public IList<SpecificationOption> Tranmission { get; set; } = new List<SpecificationOption>();
    public IList<SpecificationOption> DriveTrain { get; set; } = new List<SpecificationOption>();
    public IList<SpecificationOption> Color { get; set; } = new List<SpecificationOption>();
    public string SpecificationAttributeOptionIdColorOptionId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public enum AdType
{
    [Description("I am a private seller")] Private,

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