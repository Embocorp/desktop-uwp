using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MultiporterC
{
    [XmlInclude(typeof(DataChartNode))]
    [XmlInclude(typeof(VariableNode))]
    [XmlInclude(typeof(QuantitativeRelationshipNode))]
    [XmlInclude(typeof(MaterialNode))]

    public class ExperimentNode:BaseClass
    {
        public ExperimentNode(String name, String content, String cat)
        {
            Name = name;
            Content = content;
            Category = cat;
        }

        public ExperimentNode() { }

        public string Name { set; get; }
        public string Content { set; get; }
        public string Category { set; get; }
        public string Description { set; get; }
    }

    public class DataChartNode : ExperimentNode
    {
        private string[] chartNames = { "Line", "Column", "Bar Graph", "Scatter Plot" };
        public DataChartNode(String title, ChartType type)
        {
            Type = type;
            Title = title;
            Category = "Data Analysis";
        }

        public DataChartNode(ChartType type)
        {
            Type = type;
            Category = "Data Analysis";
            Name = chartNames[(int)type];
            Title = "";
            Length = 0;
            Data = new List<DataPoint>();
        }

        public DataChartNode() { }

        public void Add_Data_Point(DataPoint d)
        {
            Data.Add(d);
            Length++;
        }

        public void Add_Data_Set(DataPoint[] d)
        {
            foreach (DataPoint dp in d)
            {
                Data.Add(dp);
                Length++;
            }
        }

        public VariableNode Independent { get; set; }
        public VariableNode Dependent { get; set; }
        public enum ChartType { Line, Column, Bar, Scatter }
        public ChartType Type { set; get; }
        public string Title { set; get; }
        public int Length { set; get; }
        public List<DataPoint> Data { set; get; }
        public string Y_Axis_Name
        {
            get
            {
                if (Dependent == null)
                {
                    return "Y-Axis Label";
                }
                else
                {
                    return Dependent.Measurement_Name + " (" + Dependent.Unit_Name + ")";
                }
            }
        }
        public string X_Axis_Name
        {
            get
            {
                if (Independent == null)
                {
                    return "X-Axis Label";
                }
                else
                {
                    return Independent.Measurement_Name + " (" + Independent.Unit_Name + ")";
                }
            }
        }
    }

    public class VariableNode : ExperimentNode
    {
        public VariableNode() { }

        public VariableNode(VariableType type, Measurement m)
        {
            Type = type;
            Name = name_ref[(int)Type];
            Measure = m;
            Category = "Problem";
            Unit = -1;
        }

        public VariableNode(VariableType type)
        {
            Type = type;
            Name = name_ref[(int)Type];
            Category = "Problem";
            Unit = -1;
        }

        private string[] name_ref = { "Independent Variable", "Dependent Variable", "Controlled Variable" };
        public enum VariableType { Independent, Dependent, Controlled };
        public VariableType Type { get; set; }
        public Measurement Measure { get; set; }
        public int Unit { get; set; }

        [XmlIgnoreAttribute]
        public Measurement MeasureSource
        {
            get { return this.Measure; }
            set
            {
                this.Measure = value;
                OnPropertyChanged("Measurement_Name");
                OnPropertyChanged("Unit_Exists");
                OnPropertyChanged("MF");
            }
        }
        [XmlIgnoreAttribute]
        public int UnitSource
        {
            set
            {
                Unit = value;
                OnPropertyChanged("Unit_Name");
                OnPropertyChanged("UF");
            }
        }
        [XmlIgnoreAttribute]
        public string Measurement_Name
        {
            get
            {
                if (Measure == null) { return "Select a Measurement"; }
                else { return Measure.Name; }
            }
        }
        [XmlIgnoreAttribute]
        public string Unit_Name
        {
            get
            {
                if (Measure == null) { return "No measurement"; }
                else if (Unit < 0) { return "Select a unit"; }
                else { return Measure.Unit[Unit]; }
            }
        }
        [XmlIgnoreAttribute]
        public SolidColorBrush MF
        {
            get
            {
                if (Measure == null) { return new SolidColorBrush(Color.FromArgb(0xFF, 0x70, 0x70, 0x70)); }
                else { return new SolidColorBrush(Colors.Black); }
            }
        }
        [XmlIgnoreAttribute]
        public SolidColorBrush UF
        {
            get
            {
                if (Measure == null || Unit < 0) { return new SolidColorBrush(Color.FromArgb(0xFF, 0x70, 0x70, 0x70)); }
                else { return new SolidColorBrush(Colors.Black); }
            }
        }
        [XmlIgnoreAttribute]
        public bool Unit_Exists
        {
            get { return Measure != null; }
        }

    }

    public class QuantitativeRelationshipNode : ExperimentNode
    {
        private string[] names = new string[] { "Correlation", "Function", "Statistics" };
        private string[][] subtype_names = new string[][]
        {
            new string[]{ "Positive", "Negative", "Constant" },
            new string[]{ "Linear", "Power","Logarithmic", "Exponential", "Inverse", "Cyclic"},
            new string[]{ "No significance", "Significance" }
        };
        private string[][] subtype_images = new string[][]
        {
            new string[]{ "Positive", "Negative", "Constant" },
            new string[]{ "Linear", "Power","Logarithmic", "Exponential", "Inverse", "Cyclic"},
            new string[]{ "No significance", "Significance" }
        };
        private string[] type_description = new string[]
        {
            "The relationship between the two variables is linear, meaning there is a constant " +
            "relationship between the two variables.",
            "The relationship between the two variables follows a specific function.",
            "The relationship between the two variables has a statistical relationship."
        };
        private string[][] subtype_descriptions = new string[][]
        {
            new string[]
            {
                "The variables are directly related. As the independent variable quantity increases, " +
                "the dependent variable's quantity increases.",
                "The variables are indirectly related.  As the independent variable quantity increases," +
                " the dependent variable quantity decreases.",
                "The dependent variable remains constant for as the independent variable changes."
            },
            new string[]
            {
                "There is a constant relationship between the independent and dependent variable",
                "The dependent variable increases as a power of a constant value as the independent variable increases (dep = c^(ind)).",
                "The dependent variable changes in accordance to a logarithmic scale in relation to the independent variable (dep = log(ind)).",
                "The dependent variable increases exponentially with the independent variable (dep = ind^c).", 
                "As the independent variable increases, the dependent variable decreases over a set rate (dep = k/ind).",
                "The dependent variable increases and decreases according to a specific model repeatedly (dep = cos(ind))."
            },
            new string[]
            {
                "Variation in the dependent variable can be attributed to changes in the independent variable, and not by random chance.",
                "Variation in the dependent variable is just be chance."
            },

        };
        private Uri[][] rep_images = new Uri[][]
        {
            new Uri[]
            {
                new Uri("ms-appx:///Images/linear.png"),
                new Uri("ms-appx:///Images/negative.png"),
                new Uri("ms-appx:///Images/constant.png")
            },
            new Uri[]
            {
                new Uri("ms-appx:///Images/linear.png"),
                new Uri("ms-appx:///Images/negative.png"),
                new Uri("ms-appx:///Images/constant.png"),
                new Uri("ms-appx:///Images/linear.png"),
                new Uri("ms-appx:///Images/negative.png"),
                new Uri("ms-appx:///Images/constant.png")
            },
            new Uri[]
            {
                new Uri("ms-appx:///Images/linear.png"),
                new Uri("ms-appx:///Images/negative.png")
            }
        };

        public QuantitativeRelationshipNode () { }

        public QuantitativeRelationshipNode (string junk)
        {
            Name = "Quantiative Relationship";
            Type = QuantType.Unset;
            Category = "Hypothesis";
            Subtype = -1;
        }

        public enum QuantType { Correlation, Function, Statistical, Unset }
        public QuantType Type { get; set; }
        public int Subtype { get; set; }

        [XmlIgnoreAttribute]
        public Uri Rep {
            get
            {
                Debug.WriteLine(ImageVisible);
                if (ImageVisible == Visibility.Visible)
                {
                    Debug.WriteLine(rep_images[(int)Type][(int)Subtype]);
                    return rep_images[(int)Type][(int)Subtype];
                }
                return new Uri("appx:/Images/linear.png");
            }
        }
        [XmlIgnoreAttribute]
        public string Definition
        {
            get
            {
                if (Subtype > -1)
                {
                    return type_description[(int)Type] + "\n\n" + subtype_descriptions[(int)Type][Subtype];
                }
                else if (Type == QuantType.Unset)
                {
                    return "Choose a type and see its description";
                } 
                return type_description[(int)Type];
            }
        }
        [XmlIgnoreAttribute]
        public Visibility DefinitionVisibility
        {
            get
            {
                if (SubtypeEnabled)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
        [XmlIgnoreAttribute]
        public string TypeString
        {
            get
            {
                if (Type == QuantType.Unset) { return "Choose a type";  }
                return names[(int)Type];
            }
        }
        [XmlIgnoreAttribute]
        public string SubtypeString
        {
            get
            {
                if (Subtype < 0) { return "Choose a subtype"; }
                return subtype_names[(int)Type][Subtype];
            }
        }
        [XmlIgnoreAttribute]
        public QuantType TypeSource
        {
            set
            {
                Type = value;
                Subtype = -1;
                OnPropertyChanged("TypeString");
                OnPropertyChanged("SubtypeEnabled");
                OnPropertyChanged("ImageVisible");
                OnPropertyChanged("SubtypeString");
                OnPropertyChanged("Definition");
                OnPropertyChanged("DefinitionVisible");
                OnPropertyChanged("Rep");
            }
        }
        [XmlIgnoreAttribute]
        public int SubtypeSource
        {
            set
            {
                Subtype = value;
                OnPropertyChanged("SubtypeString");
                OnPropertyChanged("ImageVisible");
                OnPropertyChanged("Rep");
                OnPropertyChanged("Definition");
            }
        }
        [XmlIgnoreAttribute]
        public bool SubtypeEnabled
        {
            get
            {
                return Type != QuantType.Unset;
            }
        }
        [XmlIgnoreAttribute]
        public Visibility ImageVisible
        {
            get
            {
                if (Subtype < 0) { return Visibility.Collapsed; }
                return Visibility.Visible;
            }
        }
        [XmlIgnoreAttribute]
        public string[][] SubtypeNames
        {
            get
            {
                return subtype_names;
            }
        }
    }

    public class MaterialNode : ExperimentNode {
        public MaterialNode() { }
        public MaterialNode(string junk) {
            Category = "Materials";
            Name = "Material";
            GetImage();
        }

        private BitmapImage _source;
        private String _image;

        public async void GetImage ()
        {
            if (MImage == null || MImage == "")
            {
                B = new BitmapImage();
            }
            else
            {
                byte[] array = Convert.FromBase64String(MImage);
                using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    using (DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0)))
                    {
                        writer.WriteBytes(array);
                        await writer.StoreAsync();
                    }
                    BitmapImage image = new BitmapImage();
                    await image.SetSourceAsync(stream);
                    B = image;
                    _source = image;
                }
            }
        }

        public async Task AddImage(StorageFile f)
        {
            Stream x = await f.OpenStreamForReadAsync();
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = x.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                MImage = Convert.ToBase64String(ms.ToArray());
            }
        }

        public string MaterialName { get; set; }
        public string MaterialDescription { get; set; }
        public string MImage
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                GetImage();
            }
        }
        public int Quantity { get; set; }

        [XmlIgnoreAttribute]
        public string QuantityString
        {
            get
            {
                return Quantity.ToString();
            }
            set
            {
                Quantity = int.Parse(value);
            }
        }
        [XmlIgnoreAttribute]
        public BitmapImage B
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
                OnPropertyChanged("B");
            }
        }
    }
}
