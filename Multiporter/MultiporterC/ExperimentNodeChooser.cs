using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiporterC
{
    public class ExperimentNodeChooser
    {
        public ExperimentNodeChooser() { }
        public static NodeType[] GetNodeList(string sel)
        {
            NodeType[] items;
            if (sel.Equals("Problem"))
            {
                items = new NodeType[]
                {
                    new NodeType("Background",
                        new ExperimentNode("Background", "", sel),
                        "The background contains all pertinent information related to your investigation.  Often it is the information which inspired you to start the experiment or information that is crtical to known before starting the experiment."),
                    new NodeType("Experimental Problem", 
                        new ExperimentNode("Experimental Problem", "", sel),
                        "The main problem or question that your experiment is trying to get an answer to."),
                    new NodeType("Independent Variable", 
                        new VariableNode(VariableNode.VariableType.Independent),
                        "The quantity that you will change during your experiment to cause a change in the Dependent Variable."),
                    new NodeType("Dependent Variable", 
                        new VariableNode(VariableNode.VariableType.Dependent),
                        "The quantity that you want to measure that will change in response to your Independent Variable.  Changes in this variable " +
                        "should provide an answer to your problem."),
                    new NodeType("Controlled Variable", 
                        new VariableNode(VariableNode.VariableType.Controlled),
                        "Any quantity that should not change as a result of your testing."),
                };
            }
            else if (sel.Equals("Hypothesis") || sel.Equals("Conclusions"))
            {
                items = new NodeType[]
                {
                    new NodeType("Relationship", 
                        new ExperimentNode("Relationship", "", sel),
                        "What is the possible relationship between your independent and dependent variable."),
                    new NodeType("Quantitative Relationship", 
                        new QuantitativeRelationshipNode("junk"),
                        "The mathematical relationship between your variables. For example, if the independent variable " +
                        "increases, the dependent variable should increase too."),
                    new NodeType("Qualitative Causality", 
                        new ExperimentNode("Qualitative Causality", "", sel),
                        "The non-mathematic relationship between your variables (as temperature increases, the color of the water should become more blue).")
                };
            }
            else if (sel.Equals("Materials"))
            {
                items = new NodeType[]
                {
                    new NodeType("Material", 
                        new MaterialNode("junk"),
                        "An object or tool used for your experiment")
                };
            }
            else if (sel.Equals("Experiment"))
            {
                items = new NodeType[]
                {
                    new NodeType("Trials", 
                        new ExperimentNode("Trials", "", sel),
                        "How may times will you repeat your experiment?  The more trials performed, the more accurate your data will be."),
                    new NodeType("Procedure", 
                        new ExperimentNode("Procedure", "", sel),
                        "The steps you will take to perform your experiment."),
                    new NodeType("Measurement Methods", 
                        new ExperimentNode("Measurement", "", sel),
                        "The method that is employed to measure a variable.\n" +
                        "Independent Variables can be measured discretely (using a specific number of experimental groups, where " +
                        "the independent variable is manipulated between groups) or continuously (taking continuous measurement of a single group).\n" +
                        "Dependent Variables can be measured continuously (using continuous measurements), incrementally (measured over constant" +
                        " time intervals), finally (a single measurement take at the end of the experiment) or qualitatively (without taking any numerical measurements).")
                };
            }
            else if (sel.Equals("Data Analysis"))
            {
                items = new NodeType[]
                {
                        new NodeType("Line Chart", 
                            new DataChartNode(DataChartNode.ChartType.Line),
                            "A type of graph good for showing the change of related continuous data."),
                        new NodeType("Column Chart", 
                            new DataChartNode(DataChartNode.ChartType.Column),
                            "A type of graph good for showing the final result of a dependent variable"),
                        new NodeType("Scatter Plot", 
                            new DataChartNode(DataChartNode.ChartType.Scatter),
                            "A type of graph good for showing the random continuous data points."),
                        new NodeType("Regression Analysis", 
                            new ExperimentNode("Regression Analysis", "", sel),
                            "A mathematical analysis to determine the mathematical relationship between data points"),
                        new NodeType("Descriptive Statistics", 
                            new ExperimentNode("Descriptive Statistics", "", sel),
                            "A description of the data using statistics (i.e. mean, median, mode, variance, standard deviation)"),
                        new NodeType("Statistic Significance", 
                            new ExperimentNode("Statistic Significance", "", sel),
                            "A test to dertermine if the two sets of data are related to each other stastically."),
                        new NodeType("Qualitative Observation", 
                            new ExperimentNode("Qualitative Observation", "", sel),
                            "Any non-numerical observations made during the experiment."),
                        new NodeType("Data Table", 
                            new ExperimentNode("Data Table", "", sel),
                            "A table of data points.  Good for showing raw data points")
                };
            }
            else
            {
                items = new NodeType[] { new NodeType("Placeholder", new ExperimentNode(), "Placeholder") };
            }
            return items;
        }
    }
}
