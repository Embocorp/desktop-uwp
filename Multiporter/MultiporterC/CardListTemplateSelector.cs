using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MultiporterC
{
    public class CardListTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TextTemplate { get; set; }
        public DataTemplate DataTemplate { get; set; }
        public DataTemplate VariableTemplate { get; set; }
        public DataTemplate QuantitativeTemplate { get; set; }
        public DataTemplate MaterialTemplate { get; set; }
        public DataTemplate TrialTemplate { get; set; }
        public DataTemplate ProcedureTemplate { get; set; }
        public DataTemplate QualitativeTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item.GetType() == typeof(ExperimentNode))
            {
                return TextTemplate;
            }
            else if (item.GetType() == typeof(DataChartNode))
            {
                return DataTemplate;
            }
            else if (item.GetType() == typeof(VariableNode))
            {
                return VariableTemplate;
            }
            else if (item.GetType() == typeof(QuantitativeRelationshipNode))
            {
                return QuantitativeTemplate;
            }
            Debug.WriteLine("No");
            return TextTemplate;
        }
    }
}
