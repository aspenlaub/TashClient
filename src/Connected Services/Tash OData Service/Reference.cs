﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generation date: 2018-10-27 17:33:15
namespace Aspenlaub.Net.GitHub.CSharp.Tash.Model
{
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
    public partial class ControllableProcess : global::Microsoft.OData.Client.BaseEntityType, global::System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// Create a new ControllableProcess object.
        /// </summary>
        /// <param name="processId">Initial value of ProcessId.</param>
        /// <param name="status">Initial value of Status.</param>
        /// <param name="confirmedAt">Initial value of ConfirmedAt.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public static ControllableProcess CreateControllableProcess(int processId, global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessStatus status, global::System.DateTimeOffset confirmedAt)
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
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
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
                this.OnPropertyChanged("ProcessId");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private int _ProcessId;
        partial void OnProcessIdChanging(int value);
        partial void OnProcessIdChanged();
        /// <summary>
        /// There are no comments for Property Title in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
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
                this.OnPropertyChanged("Title");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private string _Title;
        partial void OnTitleChanging(string value);
        partial void OnTitleChanged();
        /// <summary>
        /// There are no comments for Property LaunchCommand in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
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
                this.OnPropertyChanged("LaunchCommand");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private string _LaunchCommand;
        partial void OnLaunchCommandChanging(string value);
        partial void OnLaunchCommandChanged();
        /// <summary>
        /// There are no comments for Property Status in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessStatus Status
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
                this.OnPropertyChanged("Status");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessStatus _Status;
        partial void OnStatusChanging(global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessStatus value);
        partial void OnStatusChanged();
        /// <summary>
        /// There are no comments for Property ConfirmedAt in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
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
                this.OnPropertyChanged("ConfirmedAt");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::System.DateTimeOffset _ConfirmedAt;
        partial void OnConfirmedAtChanging(global::System.DateTimeOffset value);
        partial void OnConfirmedAtChanged();
        /// <summary>
        /// This event is raised when the value of the property is changed
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// The value of the property is changed
        /// </summary>
        /// <param name="property">property name</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        protected virtual void OnPropertyChanged(string property)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new global::System.ComponentModel.PropertyChangedEventArgs(property));
            }
        }
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
    public partial class ControllableProcessTask : global::Microsoft.OData.Client.BaseEntityType, global::System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// Create a new ControllableProcessTask object.
        /// </summary>
        /// <param name="ID">Initial value of Id.</param>
        /// <param name="processId">Initial value of ProcessId.</param>
        /// <param name="type">Initial value of Type.</param>
        /// <param name="status">Initial value of Status.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public static ControllableProcessTask CreateControllableProcessTask(global::System.Guid ID, int processId, global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessTaskType type, global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessTaskStatus status)
        {
            ControllableProcessTask controllableProcessTask = new ControllableProcessTask();
            controllableProcessTask.Id = ID;
            controllableProcessTask.ProcessId = processId;
            controllableProcessTask.Type = type;
            controllableProcessTask.Status = status;
            return controllableProcessTask;
        }
        /// <summary>
        /// There are no comments for Property Id in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
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
                this.OnPropertyChanged("Id");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::System.Guid _Id;
        partial void OnIdChanging(global::System.Guid value);
        partial void OnIdChanged();
        /// <summary>
        /// There are no comments for Property ProcessId in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
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
                this.OnPropertyChanged("ProcessId");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private int _ProcessId;
        partial void OnProcessIdChanging(int value);
        partial void OnProcessIdChanged();
        /// <summary>
        /// There are no comments for Property Type in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessTaskType Type
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
                this.OnPropertyChanged("Type");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessTaskType _Type;
        partial void OnTypeChanging(global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessTaskType value);
        partial void OnTypeChanged();
        /// <summary>
        /// There are no comments for Property ControlName in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
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
                this.OnPropertyChanged("ControlName");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private string _ControlName;
        partial void OnControlNameChanging(string value);
        partial void OnControlNameChanged();
        /// <summary>
        /// There are no comments for Property Text in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
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
                this.OnPropertyChanged("Text");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private string _Text;
        partial void OnTextChanging(string value);
        partial void OnTextChanged();
        /// <summary>
        /// There are no comments for Property Status in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessTaskStatus Status
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
                this.OnPropertyChanged("Status");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessTaskStatus _Status;
        partial void OnStatusChanging(global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessTaskStatus value);
        partial void OnStatusChanged();
        /// <summary>
        /// This event is raised when the value of the property is changed
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// The value of the property is changed
        /// </summary>
        /// <param name="property">property name</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        protected virtual void OnPropertyChanged(string property)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new global::System.ComponentModel.PropertyChangedEventArgs(property));
            }
        }
    }
    /// <summary>
    /// There are no comments for ControllableProcessStatus in the schema.
    /// </summary>
    public enum ControllableProcessStatus
    {
        Idle = 0,
        Busy = 1,
        Dead = 2
    }
    /// <summary>
    /// There are no comments for ControllableProcessTaskType in the schema.
    /// </summary>
    public enum ControllableProcessTaskType
    {
        Reset = 0,
        SelectComboItem = 1,
        PressButton = 2
    }
    /// <summary>
    /// There are no comments for ControllableProcessTaskStatus in the schema.
    /// </summary>
    public enum ControllableProcessTaskStatus
    {
        Requested = 0,
        Processing = 1,
        BadRequest = 2,
        Completed = 3
    }
    /// <summary>
    /// Class containing all extension methods
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Get an entity of type global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcess as global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessSingle specified by key from an entity set
        /// </summary>
        /// <param name="source">source entity set</param>
        /// <param name="keys">dictionary with the names and values of keys</param>
        public static global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcess> source, global::System.Collections.Generic.Dictionary<string, object> keys)
        {
            return new global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessSingle(source.Context, source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)));
        }
        /// <summary>
        /// Get an entity of type global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcess as global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessSingle specified by key from an entity set
        /// </summary>
        /// <param name="source">source entity set</param>
        /// <param name="processId">The value of processId</param>
        public static global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcess> source,
            int processId)
        {
            global::System.Collections.Generic.Dictionary<string, object> keys = new global::System.Collections.Generic.Dictionary<string, object>
            {
                { "ProcessId", processId }
            };
            return new global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessSingle(source.Context, source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)));
        }
        /// <summary>
        /// Get an entity of type global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessTask as global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessTaskSingle specified by key from an entity set
        /// </summary>
        /// <param name="source">source entity set</param>
        /// <param name="keys">dictionary with the names and values of keys</param>
        public static global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessTaskSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessTask> source, global::System.Collections.Generic.Dictionary<string, object> keys)
        {
            return new global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessTaskSingle(source.Context, source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)));
        }
        /// <summary>
        /// Get an entity of type global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessTask as global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessTaskSingle specified by key from an entity set
        /// </summary>
        /// <param name="source">source entity set</param>
        /// <param name="id">The value of id</param>
        public static global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessTaskSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessTask> source,
            global::System.Guid id)
        {
            global::System.Collections.Generic.Dictionary<string, object> keys = new global::System.Collections.Generic.Dictionary<string, object>
            {
                { "Id", id }
            };
            return new global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessTaskSingle(source.Context, source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)));
        }
    }
}
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
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public DefaultContainer(global::System.Uri serviceRoot) : 
                base(serviceRoot, global::Microsoft.OData.Client.ODataProtocolVersion.V4)
        {
            this.OnContextCreated();
            this.Format.LoadServiceModel = GeneratedEdmModel.GetInstance;
            this.Format.UseJson();
        }
        partial void OnContextCreated();
        /// <summary>
        /// There are no comments for ControllableProcesses in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public global::Microsoft.OData.Client.DataServiceQuery<global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcess> ControllableProcesses
        {
            get
            {
                if ((this._ControllableProcesses == null))
                {
                    this._ControllableProcesses = base.CreateQuery<global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcess>("ControllableProcesses");
                }
                return this._ControllableProcesses;
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::Microsoft.OData.Client.DataServiceQuery<global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcess> _ControllableProcesses;
        /// <summary>
        /// There are no comments for ControllableProcessTasks in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public global::Microsoft.OData.Client.DataServiceQuery<global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessTask> ControllableProcessTasks
        {
            get
            {
                if ((this._ControllableProcessTasks == null))
                {
                    this._ControllableProcessTasks = base.CreateQuery<global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessTask>("ControllableProcessTasks");
                }
                return this._ControllableProcessTasks;
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::Microsoft.OData.Client.DataServiceQuery<global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessTask> _ControllableProcessTasks;
        /// <summary>
        /// There are no comments for ControllableProcesses in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public void AddToControllableProcesses(global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcess controllableProcess)
        {
            base.AddObject("ControllableProcesses", controllableProcess);
        }
        /// <summary>
        /// There are no comments for ControllableProcessTasks in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public void AddToControllableProcessTasks(global::Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessTask controllableProcessTask)
        {
            base.AddObject("ControllableProcessTasks", controllableProcessTask);
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private abstract class GeneratedEdmModel
        {
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
            private static global::Microsoft.OData.Edm.IEdmModel ParsedModel = LoadModelFromString();
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
            private const string Edmx = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Aspenlaub.Net.GitHub.CSharp.Tash.Model"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""ControllableProcess"">
        <Key>
          <PropertyRef Name=""ProcessId"" />
        </Key>
        <Property Name=""ProcessId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Title"" Type=""Edm.String"" />
        <Property Name=""LaunchCommand"" Type=""Edm.String"" />
        <Property Name=""Status"" Type=""Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessStatus"" Nullable=""false"" />
        <Property Name=""ConfirmedAt"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
      </EntityType>
      <EntityType Name=""ControllableProcessTask"">
        <Key>
          <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Guid"" Nullable=""false"" />
        <Property Name=""ProcessId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Type"" Type=""Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessTaskType"" Nullable=""false"" />
        <Property Name=""ControlName"" Type=""Edm.String"" />
        <Property Name=""Text"" Type=""Edm.String"" />
        <Property Name=""Status"" Type=""Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessTaskStatus"" Nullable=""false"" />
      </EntityType>
      <EnumType Name=""ControllableProcessStatus"">
        <Member Name=""Idle"" Value=""0"" />
        <Member Name=""Busy"" Value=""1"" />
        <Member Name=""Dead"" Value=""2"" />
      </EnumType>
      <EnumType Name=""ControllableProcessTaskType"">
        <Member Name=""Reset"" Value=""0"" />
        <Member Name=""SelectComboItem"" Value=""1"" />
        <Member Name=""PressButton"" Value=""2"" />
      </EnumType>
      <EnumType Name=""ControllableProcessTaskStatus"">
        <Member Name=""Requested"" Value=""0"" />
        <Member Name=""Processing"" Value=""1"" />
        <Member Name=""BadRequest"" Value=""2"" />
        <Member Name=""Completed"" Value=""3"" />
      </EnumType>
    </Schema>
    <Schema Namespace=""Aspenlaub.Net.GitHub.CSharp.Tash"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityContainer Name=""DefaultContainer"">
        <EntitySet Name=""ControllableProcesses"" EntityType=""Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcess"" />
        <EntitySet Name=""ControllableProcessTasks"" EntityType=""Aspenlaub.Net.GitHub.CSharp.Tash.Model.ControllableProcessTask"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
            public static global::Microsoft.OData.Edm.IEdmModel GetInstance()
            {
                return ParsedModel;
            }
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
            private static global::Microsoft.OData.Edm.IEdmModel LoadModelFromString()
            {
                global::System.Xml.XmlReader reader = CreateXmlReader(Edmx);
                try
                {
                    return global::Microsoft.OData.Edm.Csdl.CsdlReader.Parse(reader);
                }
                finally
                {
                    ((global::System.IDisposable)(reader)).Dispose();
                }
            }
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
            private static global::System.Xml.XmlReader CreateXmlReader(string edmxToParse)
            {
                return global::System.Xml.XmlReader.Create(new global::System.IO.StringReader(edmxToParse));
            }
        }
    }
}
