using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace D365CodeGenerator
{
    public partial class GeneratorWindow : Window
    {
        public ObservableCollection<ContractParameter> Parameters { get; set; }

        public GeneratorWindow()
        {
            InitializeComponent();

            Parameters = new ObservableCollection<ContractParameter>
            {
                new ContractParameter { DataType = "CustAccount", Name = "custAccount", Label = "@SYS12345" }
            };
            GridParameters.ItemsSource = Parameters;
        }

        private void TxtBaseName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string baseName = TxtBaseName.Text.Trim();
            if (TxtContractName != null) TxtContractName.Text = baseName + "Contract";
            if (TxtServiceName != null) TxtServiceName.Text = baseName + "Service";
            if (TxtControllerName != null) TxtControllerName.Text = baseName + "Controller";
        }

        private void Visibility_Changed(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;

            if (TabContract != null)
                TabContract.Visibility = ChkContract.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            if (TabService != null)
                TabService.Visibility = ChkService.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            if (TabController != null)
                TabController.Visibility = ChkController.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            string contractName = TxtContractName.Text.Trim();
            string serviceName = TxtServiceName.Text.Trim();
            string controllerName = TxtControllerName.Text.Trim();

            if (string.IsNullOrEmpty(contractName) && string.IsNullOrEmpty(serviceName) && string.IsNullOrEmpty(controllerName))
            {
                MessageBox.Show("Please enter a valid Base Name.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            TabResult.Visibility = Visibility.Visible;

            if (ChkContract.IsChecked == true) TxtContractCode.Text = GenerateContractCode(contractName);
            if (ChkService.IsChecked == true) TxtServiceCode.Text = GenerateServiceCode(serviceName, contractName);
            if (ChkController.IsChecked == true) TxtControllerCode.Text = GenerateControllerCode(controllerName, serviceName);

            if (ChkContract.IsChecked == true) TabResult.SelectedItem = TabContract;
            else if (ChkService.IsChecked == true) TabResult.SelectedItem = TabService;
            else if (ChkController.IsChecked == true) TabResult.SelectedItem = TabController;
        }

        private string GenerateContractCode(string className)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[DataContractAttribute]");
            sb.AppendLine($"public class {className}");
            sb.AppendLine("{");

            foreach (var p in Parameters)
            {
                if (!string.IsNullOrWhiteSpace(p.Name) && !string.IsNullOrWhiteSpace(p.DataType))
                {
                    sb.AppendLine($"    private {p.DataType} {p.Name};");
                }
            }
            sb.AppendLine();

            foreach (var p in Parameters)
            {
                if (string.IsNullOrWhiteSpace(p.Name) || string.IsNullOrWhiteSpace(p.DataType)) continue;

                string methodSuffix = char.ToUpper(p.Name[0]) + (p.Name.Length > 1 ? p.Name.Substring(1) : "");

                if (!string.IsNullOrWhiteSpace(p.Label))
                {
                    sb.AppendLine($"    [DataMemberAttribute, SysOperationLabel(literalStr(\"{p.Label}\"))]");
                }
                else
                {
                    sb.AppendLine("    [DataMemberAttribute]");
                }

                sb.AppendLine($"    public {p.DataType} parm{methodSuffix}({p.DataType} _{p.Name} = {p.Name})");
                sb.AppendLine("    {");
                sb.AppendLine($"        {p.Name} = _{p.Name};");
                sb.AppendLine($"        return {p.Name};");
                sb.AppendLine("    }");
                sb.AppendLine();
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateServiceCode(string className, string contractName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"public class {className} extends SysOperationServiceBase");
            sb.AppendLine("{");

            if (ChkContract.IsChecked == true && !string.IsNullOrEmpty(contractName))
            {
                sb.AppendLine($"    public void process({contractName} _contract)");
                sb.AppendLine("    {");
                sb.AppendLine("        // TODO: Add your business logic here");

                if (Parameters.Count > 0)
                {
                    sb.AppendLine();
                    foreach (var p in Parameters)
                    {
                        if (!string.IsNullOrWhiteSpace(p.Name) && !string.IsNullOrWhiteSpace(p.DataType))
                        {
                            string methodSuffix = char.ToUpper(p.Name[0]) + (p.Name.Length > 1 ? p.Name.Substring(1) : "");
                            sb.AppendLine($"        // {p.DataType} _{p.Name} = _contract.parm{methodSuffix}();");
                        }
                    }
                }
                sb.AppendLine();
                sb.AppendLine("        info(\"Process completed successfully.\");");
                sb.AppendLine("    }");
            }
            else
            {
                sb.AppendLine("    public void process()");
                sb.AppendLine("    {");
                sb.AppendLine("        // TODO: Add your business logic here");
                sb.AppendLine("    }");
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateControllerCode(string className, string serviceName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"public class {className} extends SysOperationServiceController");
            sb.AppendLine("{");

            sb.AppendLine("    public void new()");
            sb.AppendLine("    {");
            if (ChkService.IsChecked == true && !string.IsNullOrEmpty(serviceName))
            {
                sb.AppendLine($"        super(classStr({serviceName}),");
                sb.AppendLine($"              methodStr({serviceName}, process),");
                sb.AppendLine("              SysOperationExecutionMode::Synchronous);");
            }
            else
            {
                sb.AppendLine("        super();");
                sb.AppendLine("        // TODO: Specify the Service and Method name");
            }
            sb.AppendLine("    }");
            sb.AppendLine();

            sb.AppendLine("    public static void main(Args _args)");
            sb.AppendLine("    {");
            sb.AppendLine($"        {className} controller = new {className}();");
            sb.AppendLine("        ");
            sb.AppendLine("        controller.parmArgs(_args);");
            sb.AppendLine($"        controller.parmDialogCaption(\"{className} operation is running...\"); // TODO: Update the dialog caption");
            sb.AppendLine("        ");
            sb.AppendLine("        controller.parmShowDialog(true);");
            sb.AppendLine("        controller.startOperation();");
            sb.AppendLine("    }");

            sb.AppendLine("}");
            return sb.ToString();
        }

        private void BtnCopyContract_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(TxtContractCode.Text);
            MessageBox.Show("Contract code copied to clipboard!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnCopyService_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(TxtServiceCode.Text);
            MessageBox.Show("Service code copied to clipboard!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnCopyController_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(TxtControllerCode.Text);
            MessageBox.Show("Controller code copied to clipboard!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public class ContractParameter
    {
        public string DataType { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
    }
}