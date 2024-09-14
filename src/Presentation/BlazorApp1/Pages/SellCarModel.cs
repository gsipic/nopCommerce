using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Pages;

public class SellCarModel
{
    public BasicInfo BasicInfo { get; set; }
    public Price Price { get; set; }
    public VehicleInformation VehicleInformation { get; set; }

    public Features Features { get; set; }

    public PhotosAndVideo PhotosAndVideo { get; set; } = new();

    public Location Location { get; set; }

    public Contacts Contacts { get; set; }
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

public class SpecificationOptionValue
{
    public string Text { get; set; }
    public string Value { get; set; }
    public bool Data { get; set; }
}

public class Price
{
    [Required] public int SellCarPrice { get; set; }
    public bool NegotiatedPrice { get; set; }
}

public class VehicleInformation
{
    public IList<SpecificationOption> Make { get; set; } = new List<SpecificationOption>();
    public string MakeSpecificationOption { get; set; } = string.Empty;
    public IList<SpecificationOption> Model { get; set; } = new List<SpecificationOption>();
    public string ModelSpecificationOption { get; set; } = string.Empty;
    public int Year { get; set; } = 0;
    public IList<SpecificationOption> YearOptions { get; set; } = new List<SpecificationOption>();
    public int? Mileage { get; set; }
    public string VIN { get; set; } = string.Empty;
    public IList<SpecificationOption> BodyType { get; set; } = new List<SpecificationOption>();
    public string BodyTypeOption { get; set; } = string.Empty;
    public IList<SpecificationOption> FuelType { get; set; } = new List<SpecificationOption>();
    public string FuelTypeOption { get; set; } = string.Empty;
    public IList<SpecificationOption> Tranmission { get; set; } = new List<SpecificationOption>();
    public string TranmissionTypeOption { get; set; } = string.Empty;
    public IList<SpecificationOption> DriveTrain { get; set; } = new List<SpecificationOption>();
    public string DriveTrainTypeOption { get; set; } = string.Empty;
    public IList<SpecificationOption> Color { get; set; } = new List<SpecificationOption>();
    public string ColorTypeOption { get; set; } = string.Empty;
  
    public string Description { get; set; } = string.Empty;
}

public class Features
{
    public IList<SpecificationOptionValue> SustavPomoći { get; set; } = new List<SpecificationOptionValue>();
    public IList<SpecificationOptionValue> SigurnostPutnika { get; set; } = new List<SpecificationOptionValue>();
    
    public IList<SpecificationOptionValue> UdobnostPutnika { get; set; } = new List<SpecificationOptionValue>();
    public IList<SpecificationOptionValue> SvjetlaiFarovi { get; set; } = new List<SpecificationOptionValue>();
    
    public IList<SpecificationOptionValue> ZaštitaOdKrađe { get; set; } = new List<SpecificationOptionValue>();
    public IList<SpecificationOptionValue> Multimedia { get; set; } = new List<SpecificationOptionValue>();
    public IList<SpecificationOptionValue> Gumeinaplatci { get; set; } = new List<SpecificationOptionValue>();
    public IList<SpecificationOptionValue> Ostalidodaci { get; set; } = new List<SpecificationOptionValue>();
}

public class PhotosAndVideo
{
    public IList<string> ImageFile { get; set; } = new List<string>();
}

public class Location
{
    public IList<SpecificationOption> Županije { get; set; } = new List<SpecificationOption>();
    public string ŽupanijeSpecificationOption { get; set; } = string.Empty;
}

public class Contacts
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public string WebSite { get; set; }

    public SocialAccount SocialAccount { get; set; }
}

public class SocialAccount
{
    public string Facebook { get; set; }
    public string Twitter { get; set; }
    public string Instagram { get; set; }
   
}

public class PostExample
{
    public int id { get; set; }

    public override string ToString()
    {
        return $"{nameof(id)}: {id}";
    }
}