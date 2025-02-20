﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generation date: 23.12.2024 23:52:20
namespace Aspenlaub.Net.GitHub.CSharp.Tash
{
    /// <summary>
    /// There are no comments for DefaultContainer in the schema.
    /// </summary>
    public partial class DefaultContainer : global::Microsoft.OData.Client.DataServiceContext
    {
        /// <summary>
        /// Initialize a new DefaultContainer object.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        public DefaultContainer(global::System.Uri serviceRoot) : 
                base(serviceRoot, global::Microsoft.OData.Client.ODataProtocolVersion.V4)
        {
            this.ResolveName = new global::System.Func<global::System.Type, string>(this.ResolveNameFromType);
            this.OnContextCreated();
            this.Format.LoadServiceModel = GeneratedEdmModel.GetInstance;
            this.Format.UseJson();
        }
        partial void OnContextCreated();
        /// <summary>
        /// Since the namespace configured for this service reference
        /// in Visual Studio is different from the one indicated in the
        /// server schema, use type-mappers to map between the two.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        protected string ResolveNameFromType(global::System.Type clientType)
        {
            return clientType.FullName;
        }
        /// <summary>
        /// There are no comments for ControllableProcesses in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        public global::Microsoft.OData.Client.DataServiceQuery<ControllableProcess> ControllableProcesses
        {
            get
            {
                if ((this._ControllableProcesses == null))
                {
                    this._ControllableProcesses = base.CreateQuery<ControllableProcess>("ControllableProcesses");
                }
                return this._ControllableProcesses;
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        private global::Microsoft.OData.Client.DataServiceQuery<ControllableProcess> _ControllableProcesses;
        /// <summary>
        /// There are no comments for ControllableProcessTasks in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        public global::Microsoft.OData.Client.DataServiceQuery<ControllableProcessTask> ControllableProcessTasks
        {
            get
            {
                if ((this._ControllableProcessTasks == null))
                {
                    this._ControllableProcessTasks = base.CreateQuery<ControllableProcessTask>("ControllableProcessTasks");
                }
                return this._ControllableProcessTasks;
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        private global::Microsoft.OData.Client.DataServiceQuery<ControllableProcessTask> _ControllableProcessTasks;
        /// <summary>
        /// There are no comments for ControllableProcesses in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        public void AddToControllableProcesses(ControllableProcess controllableProcess)
        {
            base.AddObject("ControllableProcesses", controllableProcess);
        }
        /// <summary>
        /// There are no comments for ControllableProcessTasks in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        public void AddToControllableProcessTasks(ControllableProcessTask controllableProcessTask)
        {
            base.AddObject("ControllableProcessTasks", controllableProcessTask);
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        private abstract class GeneratedEdmModel
        {
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
            private static global::Microsoft.OData.Edm.IEdmModel ParsedModel = LoadModelFromString();
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
            private const string Edmx = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Aspenlaub.Net.GitHub.CSharp.Tash"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""ControllableProcess"">
        <Key>
          <PropertyRef Name=""ProcessId"" />
        </Key>
        <Property Name=""ProcessId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Title"" Type=""Edm.String"" />
        <Property Name=""LaunchCommand"" Type=""Edm.String"" />
        <Property Name=""Status"" Type=""Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcessStatus"" Nullable=""false"" />
        <Property Name=""ConfirmedAt"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
      </EntityType>
      <EntityType Name=""ControllableProcessTask"">
        <Key>
          <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Guid"" Nullable=""false"" />
        <Property Name=""ProcessId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Type"" Type=""Edm.String"" />
        <Property Name=""ControlName"" Type=""Edm.String"" />
        <Property Name=""Text"" Type=""Edm.String"" />
        <Property Name=""Status"" Type=""Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcessTaskStatus"" Nullable=""false"" />
        <Property Name=""ErrorMessage"" Type=""Edm.String"" />
      </EntityType>
      <EnumType Name=""ControllableProcessStatus"">
        <Member Name=""Idle"" Value=""0"" />
        <Member Name=""Busy"" Value=""1"" />
        <Member Name=""Dead"" Value=""2"" />
        <Member Name=""DoesNotExist"" Value=""3"" />
      </EnumType>
      <EnumType Name=""ControllableProcessTaskStatus"">
        <Member Name=""Requested"" Value=""0"" />
        <Member Name=""Processing"" Value=""1"" />
        <Member Name=""BadRequest"" Value=""2"" />
        <Member Name=""Completed"" Value=""3"" />
        <Member Name=""Failed"" Value=""4"" />
      </EnumType>
      <EntityContainer Name=""DefaultContainer"">
        <EntitySet Name=""ControllableProcesses"" EntityType=""Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcess"" />
        <EntitySet Name=""ControllableProcessTasks"" EntityType=""Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcessTask"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
            public static global::Microsoft.OData.Edm.IEdmModel GetInstance()
            {
                return ParsedModel;
            }
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
            private static global::Microsoft.OData.Edm.IEdmModel LoadModelFromString()
            {
                global::System.Xml.XmlReader reader = CreateXmlReader(Edmx);
                try
                {
                    global::System.Collections.Generic.IEnumerable<global::Microsoft.OData.Edm.Validation.EdmError> errors;
                    global::Microsoft.OData.Edm.IEdmModel edmModel;
                    
                    if (!global::Microsoft.OData.Edm.Csdl.CsdlReader.TryParse(reader, false, out edmModel, out errors))
                    {
                        global::System.Text.StringBuilder errorMessages = new global::System.Text.StringBuilder();
                        foreach (var error in errors)
                        {
                            errorMessages.Append(error.ErrorMessage);
                            errorMessages.Append("; ");
                        }
                        throw new global::System.InvalidOperationException(errorMessages.ToString());
                    }

                    return edmModel;
                }
                finally
                {
                    ((global::System.IDisposable)(reader)).Dispose();
                }
            }
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
            private static global::System.Xml.XmlReader CreateXmlReader(string edmxToParse)
            {
                return global::System.Xml.XmlReader.Create(new global::System.IO.StringReader(edmxToParse));
            }
        }
    }
    /// <summary>
    /// There are no comments for ControllableProcessSingle in the schema.
    /// </summary>
    public partial class ControllableProcessSingle : global::Microsoft.OData.Client.DataServiceQuerySingle<ControllableProcess>
    {
        /// <summary>
        /// Initialize a new ControllableProcessSingle object.
        /// </summary>
        public ControllableProcessSingle(global::Microsoft.OData.Client.DataServiceContext context, string path)
            : base(context, path) {}

        /// <summary>
        /// Initialize a new ControllableProcessSingle object.
        /// </summary>
        public ControllableProcessSingle(global::Microsoft.OData.Client.DataServiceContext context, string path, bool isComposable)
            : base(context, path, isComposable) {}

        /// <summary>
        /// Initialize a new ControllableProcessSingle object.
        /// </summary>
        public ControllableProcessSingle(global::Microsoft.OData.Client.DataServiceQuerySingle<ControllableProcess> query)
            : base(query) {}

    }
    /// <summary>
    /// There are no comments for ControllableProcess in the schema.
    /// </summary>
    /// <KeyProperties>
    /// ProcessId
    /// </KeyProperties>
    [global::Microsoft.OData.Client.Key("ProcessId")]
    public partial class ControllableProcess : global::Microsoft.OData.Client.BaseEntityType
    {
        /// <summary>
        /// Create a new ControllableProcess object.
        /// </summary>
        /// <param name="processId">Initial value of ProcessId.</param>
        /// <param name="status">Initial value of Status.</param>
        /// <param name="confirmedAt">Initial value of ConfirmedAt.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        public static ControllableProcess CreateControllableProcess(int processId, global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcessStatus status, global::System.DateTimeOffset confirmedAt)
        {
            ControllableProcess controllableProcess = new ControllableProcess();
            controllableProcess.ProcessId = processId;
            controllableProcess.Status = status;
            controllableProcess.ConfirmedAt = confirmedAt;
            return controllableProcess;
        }
        /// <summary>
        /// There are no comments for Property ProcessId in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        public int ProcessId
        {
            get
            {
                return this._ProcessId;
            }
            set
            {
                this.OnProcessIdChanging(value);
                this._ProcessId = value;
                this.OnProcessIdChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        private int _ProcessId;
        partial void OnProcessIdChanging(int value);
        partial void OnProcessIdChanged();
        /// <summary>
        /// There are no comments for Property Title in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        public string Title
        {
            get
            {
                return this._Title;
            }
            set
            {
                this.OnTitleChanging(value);
                this._Title = value;
                this.OnTitleChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        private string _Title;
        partial void OnTitleChanging(string value);
        partial void OnTitleChanged();
        /// <summary>
        /// There are no comments for Property LaunchCommand in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        public string LaunchCommand
        {
            get
            {
                return this._LaunchCommand;
            }
            set
            {
                this.OnLaunchCommandChanging(value);
                this._LaunchCommand = value;
                this.OnLaunchCommandChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        private string _LaunchCommand;
        partial void OnLaunchCommandChanging(string value);
        partial void OnLaunchCommandChanged();
        /// <summary>
        /// There are no comments for Property Status in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        public global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcessStatus Status
        {
            get
            {
                return this._Status;
            }
            set
            {
                this.OnStatusChanging(value);
                this._Status = value;
                this.OnStatusChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        private global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcessStatus _Status;
        partial void OnStatusChanging(global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcessStatus value);
        partial void OnStatusChanged();
        /// <summary>
        /// There are no comments for Property ConfirmedAt in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        public global::System.DateTimeOffset ConfirmedAt
        {
            get
            {
                return this._ConfirmedAt;
            }
            set
            {
                this.OnConfirmedAtChanging(value);
                this._ConfirmedAt = value;
                this.OnConfirmedAtChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        private global::System.DateTimeOffset _ConfirmedAt;
        partial void OnConfirmedAtChanging(global::System.DateTimeOffset value);
        partial void OnConfirmedAtChanged();
    }
    /// <summary>
    /// There are no comments for ControllableProcessTaskSingle in the schema.
    /// </summary>
    public partial class ControllableProcessTaskSingle : global::Microsoft.OData.Client.DataServiceQuerySingle<ControllableProcessTask>
    {
        /// <summary>
        /// Initialize a new ControllableProcessTaskSingle object.
        /// </summary>
        public ControllableProcessTaskSingle(global::Microsoft.OData.Client.DataServiceContext context, string path)
            : base(context, path) {}

        /// <summary>
        /// Initialize a new ControllableProcessTaskSingle object.
        /// </summary>
        public ControllableProcessTaskSingle(global::Microsoft.OData.Client.DataServiceContext context, string path, bool isComposable)
            : base(context, path, isComposable) {}

        /// <summary>
        /// Initialize a new ControllableProcessTaskSingle object.
        /// </summary>
        public ControllableProcessTaskSingle(global::Microsoft.OData.Client.DataServiceQuerySingle<ControllableProcessTask> query)
            : base(query) {}

    }
    /// <summary>
    /// There are no comments for ControllableProcessTask in the schema.
    /// </summary>
    /// <KeyProperties>
    /// Id
    /// </KeyProperties>
    [global::Microsoft.OData.Client.Key("Id")]
    public partial class ControllableProcessTask : global::Microsoft.OData.Client.BaseEntityType
    {
        /// <summary>
        /// Create a new ControllableProcessTask object.
        /// </summary>
        /// <param name="ID">Initial value of Id.</param>
        /// <param name="processId">Initial value of ProcessId.</param>
        /// <param name="status">Initial value of Status.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        public static ControllableProcessTask CreateControllableProcessTask(global::System.Guid ID, int processId, global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcessTaskStatus status)
        {
            ControllableProcessTask controllableProcessTask = new ControllableProcessTask();
            controllableProcessTask.Id = ID;
            controllableProcessTask.ProcessId = processId;
            controllableProcessTask.Status = status;
            return controllableProcessTask;
        }
        /// <summary>
        /// There are no comments for Property Id in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        public global::System.Guid Id
        {
            get
            {
                return this._Id;
            }
            set
            {
                this.OnIdChanging(value);
                this._Id = value;
                this.OnIdChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        private global::System.Guid _Id;
        partial void OnIdChanging(global::System.Guid value);
        partial void OnIdChanged();
        /// <summary>
        /// There are no comments for Property ProcessId in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        public int ProcessId
        {
            get
            {
                return this._ProcessId;
            }
            set
            {
                this.OnProcessIdChanging(value);
                this._ProcessId = value;
                this.OnProcessIdChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        private int _ProcessId;
        partial void OnProcessIdChanging(int value);
        partial void OnProcessIdChanged();
        /// <summary>
        /// There are no comments for Property Type in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        public string Type
        {
            get
            {
                return this._Type;
            }
            set
            {
                this.OnTypeChanging(value);
                this._Type = value;
                this.OnTypeChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        private string _Type;
        partial void OnTypeChanging(string value);
        partial void OnTypeChanged();
        /// <summary>
        /// There are no comments for Property ControlName in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        public string ControlName
        {
            get
            {
                return this._ControlName;
            }
            set
            {
                this.OnControlNameChanging(value);
                this._ControlName = value;
                this.OnControlNameChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        private string _ControlName;
        partial void OnControlNameChanging(string value);
        partial void OnControlNameChanged();
        /// <summary>
        /// There are no comments for Property Text in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        public string Text
        {
            get
            {
                return this._Text;
            }
            set
            {
                this.OnTextChanging(value);
                this._Text = value;
                this.OnTextChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        private string _Text;
        partial void OnTextChanging(string value);
        partial void OnTextChanged();
        /// <summary>
        /// There are no comments for Property Status in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        public global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcessTaskStatus Status
        {
            get
            {
                return this._Status;
            }
            set
            {
                this.OnStatusChanging(value);
                this._Status = value;
                this.OnStatusChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        private global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcessTaskStatus _Status;
        partial void OnStatusChanging(global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcessTaskStatus value);
        partial void OnStatusChanged();
        /// <summary>
        /// There are no comments for Property ErrorMessage in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        public string ErrorMessage
        {
            get
            {
                return this._ErrorMessage;
            }
            set
            {
                this.OnErrorMessageChanging(value);
                this._ErrorMessage = value;
                this.OnErrorMessageChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.7.0")]
        private string _ErrorMessage;
        partial void OnErrorMessageChanging(string value);
        partial void OnErrorMessageChanged();
    }
    /// <summary>
    /// There are no comments for ControllableProcessStatus in the schema.
    /// </summary>
    public enum ControllableProcessStatus
    {
        Idle = 0,
        Busy = 1,
        Dead = 2,
        DoesNotExist = 3
    }
    /// <summary>
    /// There are no comments for ControllableProcessTaskStatus in the schema.
    /// </summary>
    public enum ControllableProcessTaskStatus
    {
        Requested = 0,
        Processing = 1,
        BadRequest = 2,
        Completed = 3,
        Failed = 4
    }
    /// <summary>
    /// Class containing all extension methods
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Get an entity of type global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcess as global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcessSingle specified by key from an entity set
        /// </summary>
        /// <param name="_source">source entity set</param>
        /// <param name="keys">dictionary with the names and values of keys</param>
        public static global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcessSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcess> _source, global::System.Collections.Generic.IDictionary<string, object> keys)
        {
            return new global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcessSingle(_source.Context, _source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, keys)));
        }
        /// <summary>
        /// Get an entity of type global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcess as global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcessSingle specified by key from an entity set
        /// </summary>
        /// <param name="_source">source entity set</param>
        /// <param name="processId">The value of processId</param>
        public static global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcessSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcess> _source,
            int processId)
        {
            global::System.Collections.Generic.IDictionary<string, object> keys = new global::System.Collections.Generic.Dictionary<string, object>
            {
                { "ProcessId", processId }
            };
            return new global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcessSingle(_source.Context, _source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, keys)));
        }
        /// <summary>
        /// Get an entity of type global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcessTask as global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcessTaskSingle specified by key from an entity set
        /// </summary>
        /// <param name="_source">source entity set</param>
        /// <param name="keys">dictionary with the names and values of keys</param>
        public static global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcessTaskSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcessTask> _source, global::System.Collections.Generic.IDictionary<string, object> keys)
        {
            return new global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcessTaskSingle(_source.Context, _source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, keys)));
        }
        /// <summary>
        /// Get an entity of type global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcessTask as global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcessTaskSingle specified by key from an entity set
        /// </summary>
        /// <param name="_source">source entity set</param>
        /// <param name="id">The value of id</param>
        public static global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcessTaskSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcessTask> _source,
            global::System.Guid id)
        {
            global::System.Collections.Generic.IDictionary<string, object> keys = new global::System.Collections.Generic.Dictionary<string, object>
            {
                { "Id", id }
            };
            return new global::Aspenlaub.Net.GitHub.CSharp.Tash.ControllableProcessTaskSingle(_source.Context, _source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, keys)));
        }
    }
}
