# D365 Code Generator for Visual Studio 2022

**D365 Code Generator** is a lightweight, platform-independent Visual Studio 2022 extension designed to accelerate X++ development for Microsoft Dynamics 365 Finance & Operations. It instantly generates standardized, production-ready `SysOperation` framework code (Contract, Service, Controller) without relying on local Dynamics environment dependencies.

---

## 🌟 Key Features

- **⚡ Instant Boilerplate Generation:** Create full SysOperation patterns in seconds.
- **🌐 100% Platform Independent:** Operates purely inside Visual Studio 2022. No dependencies on `Microsoft.Dynamics.*` local DLLs.
- **🎨 Modern WPF Interface:** Intuitive parameter entry, automatic casing formatting, and seamless clipboard management.
- **📋 Standalone Output Tabs:** Review and copy Contract, Service, and Controller classes independently.
- **🎯 Dynamic Code Customization:** Automatically handles DataMember attributes, SysOperation labels, typed parameters, and synchronous execution modes.

---

## 🚀 Getting Started

### Installation
1. Open Visual Studio 2022.
2. Go to **Extensions > Manage Extensions**.
3. Search for **D365 Code Generator** and click **Download**.
4. Restart Visual Studio to complete the installation.

### Usage
1. Open Visual Studio and navigate to the top menu: **Extensions > SysOperation Builder...**
2. Enter your **Base Name** (e.g., `CustUpdateProcess`).
   - The Contract, Service, and Controller class names will auto-populate (`CustUpdateProcessContract`, `CustUpdateProcessService`, `CustUpdateProcessController`).
3. Define your Contract Parameters in the grid:
   - **Data Type / EDT:** e.g., `CustAccount`, `boolean`, `int`
   - **Variable Name:** e.g., `custAccount`
   - **Label:** e.g., `@SYS12345` or descriptive text.
4. Click **Generate Code**.
5. Switch between the output tabs (**Contract Class**, **Service Class**, **Controller Class**) and click **📋 Copy Code** to paste directly into your X++ solution.

---

### Usage: CoC Snippets (New in v1.1)

1. Open any .xpp file in the Visual Studio code editor.
2. Right-click anywhere in the code editor and select Snippet > Insert Snippet...
3. Select the D365CodeGeneratorSnippets folder.
4. Choose the desired Chain of Command template (e.g., D365 Class Extension).

### ✂️ X++ Snippet Reference (v1.1)
Included templates for rapidly building Extension classes:
  -  `cocclass `: D365 Class Extension (CoC)
  -  `coctbl`: D365 Table Extension (CoC)
  -  `cocform`: D365 Form Extension (CoC)
  - `cocfds`: D365 Form DataSource Extension (CoC)

_Example Output (Class Extension):_

```xpp
[ExtensionOf(classStr(ClassName))]
final class ClassName_Extension
{
    
}
```
---

## 🛠️ Generated Code Example

### 1. Contract Class (`CustUpdateProcessContract`)
```xpp
[DataContractAttribute]
public class CustUpdateProcessContract
{
    private CustAccount custAccount;

    [DataMemberAttribute, SysOperationLabel(literalStr("@SYS12345"))]
    public CustAccount parmCustAccount(CustAccount _custAccount = custAccount)
    {
        custAccount = _custAccount;
        return custAccount;
    }
}
```

### 2. Service Class (`CustUpdateProcessService`)
```xpp
public class CustUpdateProcessService extends SysOperationServiceBase
{
    public void process(CustUpdateProcessContract _contract)
    {
        // TODO: Add your business logic here
        // CustAccount _custAccount = _contract.parmCustAccount();

        info("Process completed successfully.");
    }
}
```

### 3. Controller Class (`CustUpdateProcessController`)
```xpp
public class CustUpdateProcessController extends SysOperationServiceController
{
    public void new()
    {
        super(classStr(CustUpdateProcessService),
              methodStr(CustUpdateProcessService, process),
              SysOperationExecutionMode::Synchronous);
    }

    public static void main(Args _args)
    {
        CustUpdateProcessController controller = new CustUpdateProcessController();
        
        controller.parmArgs(_args);
        controller.parmDialogCaption("CustUpdateProcessController operation is running...");
        
        controller.parmShowDialog(true);
        controller.startOperation();
    }
}
```

---

## 💻 Requirements

- **IDE:** Visual Studio 2022 (v17.0 or higher)
- **Framework:** .NET Framework 4.8

---

## 📄 License

Distributed under the MIT License. See `LICENSE` for more information.
