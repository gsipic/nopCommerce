using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Transactions;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentMigrator;
using LinqToDB;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Data;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;

namespace Nop.Web.Themes.SaljiDalje.Data
{
    [NopMigration("2024-07-22 14:32:00", "AddLocationMigration", MigrationProcessType.Update)]
    public class AddLocationsMigration : MigrationBase
    {
        #region Fields

        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly INopDataProvider _dataProvider;

        #endregion

        #region Ctor

        public AddLocationsMigration(
            ILanguageService languageService,
            ILocalizationService localizationService,
            ISettingService settingService,
            ISpecificationAttributeService specificationAttributeService,
            INopDataProvider dataProvider)
        {
            _languageService = languageService;
            _localizationService = localizationService;
            _settingService = settingService;
            _specificationAttributeService = specificationAttributeService;
            _dataProvider = dataProvider;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            using TransactionScope transactionScope = new TransactionScope();
            try
            {
                var SpecificationAttributeGroupId = _dataProvider.InsertEntity(
                    new SpecificationAttributeGroup {Name = "BasicInfo"}
                ).Id;

                // Stanje
                var stanjeVozila = insertSpecificationAttribute("Stanje vozila", SpecificationAttributeGroupId);
                insertSpecificationAttributeOption("Kao novo", stanjeVozila);
                insertSpecificationAttributeOption("Uredno", stanjeVozila);
                insertSpecificationAttributeOption("Puno korišteno ali ispravno", stanjeVozila);
                insertSpecificationAttributeOption("Za dijelove", stanjeVozila);
                
                // Stanje
                var prodavac = insertSpecificationAttribute("Tip prodavaca", SpecificationAttributeGroupId);
                insertSpecificationAttributeOption("Privatne osobe", prodavac);
                insertSpecificationAttributeOption("Poslovne osobe", prodavac);
                
                var price = _dataProvider.InsertEntity(
                    new SpecificationAttributeGroup {Name = "Price"}
                ).Id;

                // Pregovaranje za cijenu
                var pregovaranjeZaCijenu =
                    insertSpecificationAttribute("Pregovaranje za cijenu", price);
                insertSpecificationAttributeOption("True", pregovaranjeZaCijenu);
                insertSpecificationAttributeOption("False", pregovaranjeZaCijenu);
                
                // VehicleInformation
                var vehicleInformation = _dataProvider.InsertEntity(
                    new SpecificationAttributeGroup {Name = "Vehicle information"}
                ).Id;

                var bodyType =
                    insertSpecificationAttribute("BodyType", vehicleInformation);
                insertSpecificationAttributeOption("Limuzina", bodyType);
                insertSpecificationAttributeOption("Karavan", bodyType);
                insertSpecificationAttributeOption("SUV", bodyType);
                insertSpecificationAttributeOption("Hatchback", bodyType);
                insertSpecificationAttributeOption("Monovolumen", bodyType);
                insertSpecificationAttributeOption("Coupe", bodyType);
                insertSpecificationAttributeOption("Kombibus", bodyType);
                insertSpecificationAttributeOption("Kabriolet", bodyType);
                
                var fuelType =
                    insertSpecificationAttribute("FuelType", vehicleInformation);
                insertSpecificationAttributeOption("Diesel", fuelType);
                insertSpecificationAttributeOption("Benzin", fuelType);
                insertSpecificationAttributeOption("Hibridni", fuelType);
                insertSpecificationAttributeOption("Benzin + LPG", fuelType);
                insertSpecificationAttributeOption("Električni", fuelType);
                
                var tranmission =
                    insertSpecificationAttribute("Tranmission", vehicleInformation);
                insertSpecificationAttributeOption("Automatski", tranmission);
                insertSpecificationAttributeOption("Automatski-sekvencijski", tranmission);
                insertSpecificationAttributeOption("Mehanički", tranmission);
                insertSpecificationAttributeOption("Mehanički 4 brzine", tranmission);
                insertSpecificationAttributeOption("Mehanički 5 brzine", tranmission);
                insertSpecificationAttributeOption("Mehanički 6 brzine", tranmission);
                insertSpecificationAttributeOption("Sekvencijski", tranmission);
                
                var driveTrain =
                    insertSpecificationAttribute("DriveTrain", vehicleInformation);
                insertSpecificationAttributeOption("Prednji", driveTrain);
                insertSpecificationAttributeOption("Stražnji", driveTrain);
                insertSpecificationAttributeOption("4x4", driveTrain);
                
                // Boja
                var color = insertSpecificationAttribute("Color", vehicleInformation);
                insertSpecificationAttributeOptionColor("Siva", "#8a97a8", color);
                insertSpecificationAttributeOptionColor("Crvena", "#8a374a", color);
                insertSpecificationAttributeOptionColor("Plava", "#47476f", color);
                
                // Sustav pomoći
                var features = _dataProvider.InsertEntity(
                    new SpecificationAttributeGroup {Name = "Features"}
                ).Id;
                var exterior = insertSpecificationAttribute("Sustav pomoći", features);
                insertSpecificationAttributeOption("ABS (anti-lock brake system)", exterior);
                insertSpecificationAttributeOption("Sustav upozoravanja na umor", exterior);
                insertSpecificationAttributeOption("Prepoznavanje prometnih znakova", exterior);
                insertSpecificationAttributeOption("Električni prozori", exterior);
                insertSpecificationAttributeOption("Senzori za kišu", exterior);
                insertSpecificationAttributeOption("Prednji parkirni senzori", exterior);
                
                insertSpecificationAttributeOption("Tempomat", exterior);
                insertSpecificationAttributeOption("Pomoć pri promjeni trake", exterior);
                insertSpecificationAttributeOption("Sustav za kontrolu ograničenja brzine", exterior);
                insertSpecificationAttributeOption("Središnje zaključavanje", exterior);
                insertSpecificationAttributeOption("Električna vrata prtljažnika", exterior);
                insertSpecificationAttributeOption("Stražnji parkirni senzori", exterior);
                
                insertSpecificationAttributeOption("Prilagodljivi tempomat", exterior);
                insertSpecificationAttributeOption("Pomoć za mrtvi kut", exterior);
                insertSpecificationAttributeOption("Sustav upozorenja na udaljenost", exterior);
                insertSpecificationAttributeOption("Središnje zaključavanje bez ključa", exterior);
                insertSpecificationAttributeOption("Parking kamera", exterior);
                
                insertSpecificationAttributeOption("Pomoć pri kretanju na brdu", exterior);
                insertSpecificationAttributeOption("Pomoć pri kočenju u nuždi", exterior);
                insertSpecificationAttributeOption("Električno zrcalo", exterior);
                insertSpecificationAttributeOption("Senzori za svjetla", exterior);
                insertSpecificationAttributeOption("Kamera od 360 stupnjeva", exterior);
                
                // Sigurnost putnika
                var sigurnostPutnika = insertSpecificationAttribute("Sigurnost putnika", features);
                insertSpecificationAttributeOption("Isofix sustav", sigurnostPutnika);
                insertSpecificationAttributeOption("Prednji i bočni zračni jastuci", sigurnostPutnika);
                insertSpecificationAttributeOption("Isofix na mjestu suvozača", sigurnostPutnika);
                insertSpecificationAttributeOption("Prednji, bočni i ostali zračni jastuci", sigurnostPutnika);
                insertSpecificationAttributeOption("Zračni jastuk vozača", sigurnostPutnika);
                insertSpecificationAttributeOption("Zračni jastuk vozača i suvozača", sigurnostPutnika);
                
                // Udobnost putnika
                var udobnostPutnika = insertSpecificationAttribute("Udobnost putnika", features);
                insertSpecificationAttributeOption("Grijana sjedala", udobnostPutnika);
                insertSpecificationAttributeOption("Sklopivo suvozačko sjedalo", udobnostPutnika);
                insertSpecificationAttributeOption("Sportska sjedala", udobnostPutnika);
                insertSpecificationAttributeOption("Naslon za ruku", udobnostPutnika);
                insertSpecificationAttributeOption("Ventilacija sjedala", udobnostPutnika);
                
                // Svjetla i farovi
                var svjetlaIFarovi = insertSpecificationAttribute("Svjetla i farovi", features);
                insertSpecificationAttributeOption("Svjetla za maglu", svjetlaIFarovi);
                insertSpecificationAttributeOption("Ksenonski farovi", svjetlaIFarovi);
                insertSpecificationAttributeOption("Dnevna svjetla", svjetlaIFarovi);
                insertSpecificationAttributeOption("Bi-ksenonski farovi", svjetlaIFarovi);
                insertSpecificationAttributeOption("LED dnevna svjetla", svjetlaIFarovi);
                insertSpecificationAttributeOption("Laserski farovi", svjetlaIFarovi);
                insertSpecificationAttributeOption("LED farovi", svjetlaIFarovi);
                
                // Zaštita od krađe
                var zastitaOdKrade = insertSpecificationAttribute("Zaštita od krađe", features);
                insertSpecificationAttributeOption("Alarmni sustav", zastitaOdKrade);
                insertSpecificationAttributeOption("Blokada motora", zastitaOdKrade);
                insertSpecificationAttributeOption("Mehanička zaštita", zastitaOdKrade);
                
                // Multimedia
                var multimedia = insertSpecificationAttribute("Multimedia", features);
                insertSpecificationAttributeOption("CD player", multimedia);
                insertSpecificationAttributeOption("USB priključak", multimedia);
                insertSpecificationAttributeOption("Indukcijsko punjenje za pametni telefon", multimedia);
                insertSpecificationAttributeOption("CD izmjenjivač", multimedia);
                insertSpecificationAttributeOption("Sustav za navigaciju", multimedia);
                insertSpecificationAttributeOption("Head-up display", multimedia);
                insertSpecificationAttributeOption("Bluetooth", multimedia);
                insertSpecificationAttributeOption("Apple CarPlay", multimedia);
                insertSpecificationAttributeOption("Digitalni kokpit", multimedia);
                insertSpecificationAttributeOption("Višenamjenski upravljač", multimedia);
                insertSpecificationAttributeOption("Android Auto", multimedia);
                
                // Gume i naplatci
                var gumeinaplatci = insertSpecificationAttribute("Gume i naplatci", features);
                insertSpecificationAttributeOption("Aluminijski naplatci", gumeinaplatci);
                insertSpecificationAttributeOption("Nadzor tlaka u gumama", gumeinaplatci);
                insertSpecificationAttributeOption("Ljetne gume", gumeinaplatci);
                insertSpecificationAttributeOption("Rezervni kotač", gumeinaplatci);
                insertSpecificationAttributeOption("Zimske gume", gumeinaplatci);
                insertSpecificationAttributeOption("Gume za sve sezone", gumeinaplatci);
                
                // Ostali dodaci
                var ostaliDodaci = insertSpecificationAttribute("Ostali dodaci", features);
                insertSpecificationAttributeOption("Zračni ovjes", ostaliDodaci);
                insertSpecificationAttributeOption("Krovni nosač", ostaliDodaci);
                insertSpecificationAttributeOption("Preklopiva stražnja sjedala", ostaliDodaci);
                insertSpecificationAttributeOption("Spojka za prikolicu", ostaliDodaci);
                insertSpecificationAttributeOption("Pristupačno osobama s invaliditetom", ostaliDodaci);
                insertSpecificationAttributeOption("Pomični krov", ostaliDodaci);
                insertSpecificationAttributeOption("Korišteno kao taxi vozilo", ostaliDodaci);
                insertSpecificationAttributeOption("Panoramski krov", ostaliDodaci);
                insertSpecificationAttributeOption("Krovni prozor", ostaliDodaci);

                // Location
                var location = _dataProvider.InsertEntity(
                    new SpecificationAttributeGroup {Name = "Location"}
                ).Id;
                var zupanija = insertSpecificationAttribute("Županija", location);
                //var gradoviBjelovarskoBilogorska = insertSpecificationAttribute("Bjelovarsko_bilogorska_grad",SpecificationAttributeGroupId);
                //var naselje = insertSpecificationAttribute("Naselje",SpecificationAttributeGroupId);
                insertSpecificationAttributeOption("Bjelovarsko-bilogorska", zupanija);
                insertSpecificationAttributeOption("Brodsko-posavska", zupanija);
                insertSpecificationAttributeOption("Dubrovačko-neretvanska", zupanija);
                insertSpecificationAttributeOption("Istarska", zupanija);
                insertSpecificationAttributeOption("Karlovačka", zupanija);
                insertSpecificationAttributeOption("Koprivničko-križevačka", zupanija);
                insertSpecificationAttributeOption("Krapinsko-zagorska", zupanija);
                insertSpecificationAttributeOption("Ličko-senjska", zupanija);
                insertSpecificationAttributeOption("Međimurska", zupanija);
                insertSpecificationAttributeOption("Osječko-baranjska", zupanija);
                insertSpecificationAttributeOption("Požeško-slavonska", zupanija);
                insertSpecificationAttributeOption("Primorsko-goranska", zupanija);
                insertSpecificationAttributeOption("Sisačko-moslavačka", zupanija);
                insertSpecificationAttributeOption("Splitsko-dalmatinska", zupanija);
                insertSpecificationAttributeOption("Šibensko-kninska", zupanija);
                insertSpecificationAttributeOption("Varaždinska", zupanija);
                insertSpecificationAttributeOption("Virovitičko-podravska", zupanija);
                insertSpecificationAttributeOption("Vukovarsko-srijemska", zupanija);
                insertSpecificationAttributeOption("Zadarska", zupanija);
                insertSpecificationAttributeOption("Grad Zagreb", zupanija);
                insertSpecificationAttributeOption("Zagrebačka", zupanija);
                insertSpecificationAttributeOption("Izvan Hrvatske", zupanija);

                // Dostava
                var dostavaID = insertSpecificationAttribute("Dostava", SpecificationAttributeGroupId);
                insertSpecificationAttributeOption("Moguća dostava", dostavaID);
                insertSpecificationAttributeOption("Razgledavanje putem video poziva", dostavaID);

                // Mogućnost plaćanja
                var mogućnostPlaćanja = insertSpecificationAttribute("Mogućnost plaćanja", SpecificationAttributeGroupId);
                insertSpecificationAttributeOption("Gotovina", mogućnostPlaćanja);
                insertSpecificationAttributeOption("Kredit", mogućnostPlaćanja);
                insertSpecificationAttributeOption("Leasing", mogućnostPlaćanja);
                insertSpecificationAttributeOption("Obročno Bankovnim Karticama", mogućnostPlaćanja);
                insertSpecificationAttributeOption("Preuzimanje Leasinga", mogućnostPlaćanja);
                insertSpecificationAttributeOption("Staro za Novo", mogućnostPlaćanja);
                insertSpecificationAttributeOption("Zamjena", mogućnostPlaćanja);


                transactionScope.Complete();
                transactionScope.Dispose();
            }
            catch (TransactionException ex)
            {
                transactionScope.Dispose();
                //MessageBox.Show("Transaction Exception Occured");
            }
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            //nothing
        }

        int insertSpecificationAttribute(string s, int specificationAttributeGroupId)
        {
            return _dataProvider.InsertEntity(new SpecificationAttribute
            {
                Name = s, SpecificationAttributeGroupId = specificationAttributeGroupId
            }).Id;
        }

        int insertSpecificationAttributeOption(string s, int specificationAttributeId)
        {
            return _dataProvider.InsertEntity(new SpecificationAttributeOption
            {
                Name = s, SpecificationAttributeId = specificationAttributeId
            }).Id;
        }

        int insertSpecificationAttributeOptionColor(string name, string colorSquareRgb, int specificationAttributeId)
        {
            return _dataProvider.InsertEntity(new SpecificationAttributeOption
            {
                Name = name, ColorSquaresRgb = colorSquareRgb, SpecificationAttributeId = specificationAttributeId
            }).Id;
        }

        #endregion
    }
}