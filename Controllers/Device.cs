/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;
using System.Text;


namespace AIBot_FYP16004
{

    //Representation of Relationship entities in language model
    public enum RelationshipOptions
    {
        Unknown,
        [Terms("Greater")]
        [Describe(">")]
        Greater,

        [Terms("Smaller")]
        [Describe("<")]
        Smaller,

        [Terms("Equal")]
        Equal,

        [Terms("Without")]
        Without,

        [Terms("Approx")]
        Approx
    };

    //Representation of Usage entities in language model
    public enum UsageOptions
    {
        Unknown,
        [Terms("PlayGames")]
        PlayGames,
        [Terms("VideoEditing")]
        VideoEditing,
        [Terms("DocumentationEditing")]
        DocumentationEditing
    };

    //Representation of Product entities in language model
    public enum ProductOptions
    {
        Unknown,
        [Terms("Spectre")]
        Spectre,
        [Terms("SurfaceBook")]
        SurfaceBook,
        [Terms("SurfacePro")]
        SurfacePro,
        [Terms("Yoga")]
        Yoga,
        [Terms("Zenbook")]
        Zenbook
    };

    //Representation of Feature entities in language model
    public enum FeatureOptions
    {
        Unknown,
        [Terms("Brand")]
        Brand,
        [Terms("RAM")]
        RAM,
        [Terms("GraphicCard")]
        GraphicCard,
        [Terms("Processor")]
        Processor,
        [Terms("HardDrive")]
        HardDrive,
        [Terms("PriceValue")]
        PriceValue
    };

    public enum PreferenceOptions
    {
        Unknown,
        [Terms("yes")]
        yes,
        [Terms("no")]
        no
    };

    //Processors available
    public enum ProcessorOptions
    {
        Unknown,
        [Terms("i5")]
        i5,
        [Terms("i7")]
        i7,
        [Terms("m3")]
        m3,
        [Terms("m5")]
        m5
    };

    //Brands available
    public enum BrandOptions
    {
        Unknown,
        [Terms("Microsoft", "microsoft")]
        Microsoft,
        [Terms("Lenovo", "lenovo")]
        Lenovo,
        [Terms("ASUS", "asus")]
        ASUS,
        [Terms("HP","hp")]
        HP
    };

    //Price ranges to be picked by users
    public enum PriceRangeOptions
    {
        Unknown,
        [Describe("2000hkd~5000hkd")]
        Range1,
        [Describe("5000hkd~8000hkd")]
        Range2,
        [Describe("8000hkd~10000hkd")]
        Range3,
        [Describe("above 10000hkd")]
        Range4
    };

    //RAM ranges to be picked by users
    public enum RAMRangeOptions
    {
        Unknown,
        [Describe("<4GB")]
        Range1,
        [Describe("4GB-8GB")]
        Range2,
        [Describe("8GB-16GB")]
        Range3,
        [Describe(">16GB")]
        Range4
    };

    //Hard Disk Space ranges to be picked by users
    public enum HDRangeOptions
    {
        Unknown,
        [Describe("<250GB")]
        Range1,
        [Describe("250GB-500GB")]
        Range2,
        [Describe("500GB-800GB")]
        Range3,
        [Describe(">800GB")]
        Range4
    };
    */

    /*
    [Serializable]
    public class FeatureRequirement
    {        
        public FeatureOptions feature;
        public RelationshipOptions relation;
        public string value;
    };
    */

    /*
    //This class represents the user requirements on the device
    [Serializable]
    [Template(TemplateUsage.NotUnderstood, "I do not understand \"{0}\".", "Try again, I don't get \"{0}\".")] //Add templates to respond to unknown text
    public class DeviceOrder
    {

        [Prompt("Do you have preference on brand? {||}")]
        [Describe("Brand specified")]
        public PreferenceOptions hasBrand;
        public BrandOptions Brand_value;

        [Describe("Price specified")]
        public PreferenceOptions hasPrice;
        public RelationshipOptions price_relation;
        [Prompt("Please enter the price, in terms of HKD. Please enter the number only")]
        public string price_value;

        [Prompt("Do you have preference on range of price? {||}")]
        [Describe("Price range")]
        public PreferenceOptions needPrice;
        public PriceRangeOptions price_range;

        [Prompt("Do you have preference on Processor? {||}")]
        [Describe("Processor specified")]
        public PreferenceOptions hasProcessor;
        //public RelationshipOptions Processor_relation;
        public ProcessorOptions Processor_value;

        [Describe("RAM specified")]
        public PreferenceOptions hasRAM;
        public RelationshipOptions RAM_relation;
        [Prompt("Please enter the RAM size, in terms of GB. Please enter the number only")]
        public string RAM_value;

        [Prompt("Do you have preference on range of RAM size? {||}")]
        [Describe("RAM size range")]
        public PreferenceOptions needRAM;
        public RAMRangeOptions RAM_range;

        [Describe("Hard Disk Space specified")]
        public PreferenceOptions hasHD;
        public RelationshipOptions HD_relation;
        [Prompt("Please enter the Hard Disk Space, in terms of GB. Please enter the number only")]
        public string HD_value;

        [Prompt("Do you have preference on range of Hard Disk space? {||}")]
        [Describe("Hard Disk Space range")]
        public PreferenceOptions needHD;
        public HDRangeOptions HD_range;

        [Prompt("Do you need a Graphic Card on your device? {||}")]
        [Describe("Graphic Card needed")]
        public PreferenceOptions hasGC;

        [Optional]
        [Prompt("Do you want to specify the usage? {||}")]
        public UsageOptions usage;

        public ProductOptions model;

        //The string representation of the class
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("DeviceOrder(Usgae: {0}, Product: {1},",usage,model);
            builder.AppendFormat(" Brand: {0},", Brand_value);
            if(hasPrice==PreferenceOptions.yes)
                builder.AppendFormat(" Price: {0} {1},",price_relation,price_value);
            if (needPrice == PreferenceOptions.yes)
            {
                string description="";
                switch (price_range)
                {
                    case PriceRangeOptions.Range1:
                        description= "2000hkd~5000hkd";
                        break;
                    case PriceRangeOptions.Range2:
                        description = "5000hkd~8000hkd";
                        break;
                    case PriceRangeOptions.Range3:
                        description = "8000hkd~10000hkd";
                        break;
                    case PriceRangeOptions.Range4:
                        description = "above 10000hkd";
                        break;
                }
                builder.AppendFormat(" Price Range: {0}", description);
            }

            builder.AppendFormat(" Processor: {0},", Processor_value);

            if(hasRAM==PreferenceOptions.yes)
                builder.AppendFormat(" RAM: {0} {1},", RAM_relation, RAM_value);
            if (needRAM == PreferenceOptions.yes)
            {
                string description = "";
                switch (RAM_range)
                {
                    case RAMRangeOptions.Range1:
                        description = "<4GB";
                        break;
                    case RAMRangeOptions.Range2:
                        description = "4GB-8GB";
                        break;
                    case RAMRangeOptions.Range3:
                        description = "8GB-16GB";
                        break;
                    case RAMRangeOptions.Range4:
                        description = ">16GB";
                        break;
                }
                builder.AppendFormat(" RAM size Range: {0}", description);
            }

            if(hasHD==PreferenceOptions.yes)
                builder.AppendFormat(" Hard Disk: {0} {1},", HD_relation, HD_value);
            if(needHD==PreferenceOptions.yes)
            {
                string description = "";
                switch (HD_range)
                {
                    case HDRangeOptions.Range1:
                        description = "<250GB";
                        break;
                    case HDRangeOptions.Range2:
                        description = "250GB-500GB";
                        break;
                    case HDRangeOptions.Range3:
                        description = "500GB-800GB";
                        break;
                    case HDRangeOptions.Range4:
                        description = ">800GB";
                        break;
                }
                builder.AppendFormat(" Hard Disk Space Range: {0}", description);
            }

            builder.AppendFormat(" Graphic Card: {0},", hasGC);
            builder.Append(")");
            return builder.ToString();
        }


    };
}

*/