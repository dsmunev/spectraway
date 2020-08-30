using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Linq;
using System.Xml.Linq;

namespace SpectraWay.Localization
{

    public interface IListenStringResourceProvider
    {
        void OnPopulateStringResourceProvider();
    }
    public sealed class StringResourceProvider
    {
        private readonly string resourcePrefix = "SpectraWay.";

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static class Keys
        {

            #region XAML
            public const string UiSettings = "UiSettings";
            public const string Intensity = "Intensity";
            public const string Wavelength = "Wavelength";
            public const string ExceptionInfo = "ExceptionInfo";
            public const string CopyToClipboard = "CopyToClipboard";
            public const string Close = "Close";
            public const string LogViewer = "LogViewer";
            public const string Save = "Save";

            public const string ExperimentName = "ExperimentName";
            public const string Category = "Category";
            public const string PhysicsModel = "PhysicsModel";
            public const string Spectrometer = "Spectrometer";
            public const string WaveRange = "WaveRange";
            public const string From = "From";
            public const string nm = "nm";
            public const string To = "To";
            public const string DistanceList = "DistanceList";
            public const string BaseDistance = "BaseDistance";
            public const string Cancel = "Cancel";

            public const string CcdLevels = "CcdLevels";
            public const string SpectrometerStatus = "SpectrometerStatus";
            public const string Offline = "Offline";
            public const string Online = "Online";
            public const string Exposure = "Exposure";
            public const string EnterToSaveNowIsMs = "EnterToSaveNowIsMs";
            public const string RemoveNoise = "RemoveNoise";
            public const string Normalize = "Normalize";
            public const string Led = "Led";
            public const string CurrentDistance = "CurrentDistance";
            public const string GoToDistance = "GoToDistance";
            public const string SaveDataAs = "SaveDataAs";
            public const string CurrentL = "CurrentL";
            public const string Noise = "Noise";
            public const string SpectraForAllDistances = "SpectraForAllDistances";
            public const string ShowSavedData = "ShowSavedData";
            public const string No = "No";
            public const string Yes = "Yes";
            public const string LogScale = "LogScale";
            public const string DivideToBase = "DivideToBase";
            public const string Desc = "Desc";
            public const string NotNoise = "NotNoise";
            public const string Normalized = "Normalized";

            public const string PleaseChooseAction = "PleaseChooseAction";
            public const string ShowExperimentList = "ShowExperimentList";
            public const string NewExperiment = "NewExperiment";
            public const string Export = "Export";
            public const string Import = "Import";
            public const string About = "About";
            public const string ExperimentListIsEmpty = "ExperimentListIsEmpty";
            public const string AddNewExperiment = "AddNewExperiment";
            public const string SpectraWay = "SpectraWay";
            public const string LastOperation = "LastOperation";

            public const string ExperimentDialog = "ExperimentDialog";
            public const string SettingsDialog = "SettingsDialog";
            public const string Settings = "Settings";
            public const string Language = "Language";
            public const string SavePath = "SavePath";
            public const string Browse = "Browse";

            #endregion

            #region ViewModel

            public const string Initializing = "Initializing";
            public const string Saving = "Saving";
            public const string PleaseWait = "PleaseWait";
            public const string AllChangesAreSaved = "AllChangesAreSaved";
            public const string Experiment_PLACE_WasAdded = "Experiment_PLACE_WasAdded";//"Experiment '{0}' was added"
            public const string AttemptToSetExposureTimeTo_PLACE_Ms = "AttemptToSetExposureTimeTo_PLACE_Ms";//"Attempt to set Exposure Time to {0}ms"
            public const string ExposureTimeWasSetTo_PLACE_Ms = "ExposureTimeWasSetTo_PLACE_Ms";//"Exposure Time was set to {0}ms"
            public const string AttemptSetExposureTimeTo_PLACE_MsFailedSpectrometerNotReady = "AttemptSetExposureTimeTo_PLACE_MsFailedSpectrometerNotReady";//"Attempt to set Exposure Time to {0}ms failed, Spectrometer is not ready"
            public const string StartMovingToDistance_PLACE_Mm = "StartMovingToDistance_PLACE_Mm";//"Start moving to distance {0}mm"
            public const string DataIsEmpty = "DataIsEmpty";
            
            public const string PleaseRunSpectrometerAndTryToGetSpectralMeasurements = "PleaseRunSpectrometerAndTryToGetSpectralMeasurements";
            public const string DistanceDoesNotDefined = "DistanceDoesNotDefined";
            public const string PleaseMoveDetectorForSomeDistance = "PleaseMoveDetectorForSomeDistance";
            public const string Base = "Base";
            public const string DataAlreadyExist = "DataAlreadyExist";
            public const string DoYouWantToReplace = "DoYouWantToReplace";

            public const string StepperAssigned = "StepperAssigned";
            public const string StepperIsRestarting = "StepperIsRestarting";
            public const string StepperIsStopping = "StepperIsStopping";
            public const string CurrentDistanceIs_PLACE_Mm = "CurrentDistanceIs_PLACE_Mm";//"Current Distance is {0}mm"
            public const string StepperFailedSeeLogForDetails = "StepperFailedSeeLogForDetails";//"Stepper failed, see log for details"
            public const string SpectrometerFailedSeeLogForDetails = "SpectrometerFailedSeeLogForDetails";//"Spectrometer failed, see log for details"
            public const string SpectrometerIsRestarting = "SpectrometerIsRestarting";
            public const string SpectrometerIsStopping = "SpectrometerIsStopping";
            public const string SpectrometerAssigned = "SpectrometerAssigned";
            public const string ConnectToSpectrometerPleaseWait = "ConnectToSpectrometerPleaseWait";
            public const string SpectraForAllDistancesSuccessfullyRecieved = "SpectraForAllDistancesSuccessfullyRecieved";
            public const string RetrieveParamsPleaseWait = "RetrieveParamsPleaseWait";
            #endregion

            #region Devices
            public const string LotisSpectrometerCannotBeUsed = "LotisSpectrometerCannotBeUsed";//Lotis spectrometer cannot be used
            public const string OrminsCcdCantBeInitialize = "OrminsCcdCantBeInitialize";//Ormins CCD can`t be initialize
            public const string OrminsCcdHitTestFailed = "OrminsCcdHitTestFailed";//Ormins CCD HitTest failed
            public const string OrminsCcdHasIncorrectParams = "OrminsCcdHasIncorrectParams";//Ormins CCD has incorrect params

            public const string RequestedDistanceLessThanMinDistance = "RequestedDistanceLessThanMinDistance";//Requested Distance less than min Distance
            public const string CurrentDistanceSetAsPossibleMinimalDistance_PLACE_Mm = "CurrentDistanceSetAsPossibleMinimalDistance_PLACE_Mm";//Current Distance set as possible minimal Distance = {0}mm


            #endregion



        }

        public static readonly StringResourceProvider Instance;

        private Dictionary<string, LocalizedString> stringDictionary;

        static StringResourceProvider()
        {
            Instance = new StringResourceProvider();
            SetDefaultValues();
            //var xml = XElement.Load($"Language/Ru.xml");
            //StringResourceProvider.Instance.PopulateStringResources(xml.Elements().ToDictionary(x => x.Attribute("ElementID")?.Value, x => x.Value));

            //StringBuilder sb1 = new StringBuilder();
            //StringBuilder sb2 = new StringBuilder();

            //foreach (var a in Instance.stringDictionary)
            //{
            //    sb1.Append("<DisplayString ElementID=\"SpectraWay.");
            //    sb1.Append(a.Key);

            //    sb1.AppendLine("\">");
            //    sb1.Append("  <Name>");
            //    //sb1.Append(a.Value.Default);
            //    sb1.Append(a.Value.Value);

            //    sb1.AppendLine("</Name>");
            //    sb1.AppendLine("</DisplayString>");

            //    //<StringResource ID="Microsoft.SQLServer.Visualization.Library.StringResource.DataCenterDashboard.Home"/>
            //    sb2.Append("<StringResource ID=\"Microsoft.SQLServer.Visualization.Library.StringResource.DataCenterDashboard.");
            //    sb2.Append(a.Key);
            //    sb2.AppendLine("\"/>");
            //}
            //var ak712 = sb1.ToString();
            //var ak713 = sb2.ToString();
        }

        private StringResourceProvider()
        {
            stringDictionary = new Dictionary<string, LocalizedString>();
        }

        /// <summary>
        /// Refresh localized strings
        /// </summary>
        /// <param name="newData"></param>
        public void PopulateStringResources(IDictionary<string, string> dictionary)
        {
            if (dictionary != null)
            {
                foreach (var dataObject in dictionary)
                {
                    var tempName = dataObject.Key;
                    var tempDName = dataObject.Value;
                    if (tempName == null || tempDName == null)
                        continue;
                    var propertyName = tempName;
                    var propertyValue = tempDName;
                    if (!string.IsNullOrEmpty(propertyName) && !string.IsNullOrEmpty(propertyValue))
                    {
                        if (propertyName.StartsWith(resourcePrefix))
                        {
                            var resourceKey = propertyName.Substring(resourcePrefix.Length);
                            if (stringDictionary.ContainsKey(resourceKey))
                            {
                                stringDictionary[resourceKey].Localized = propertyValue;
                            }
                        }
                    }
                }
                SendChanges();
            }
        }

        public void SetToDefaultStringResources()
        {
            //if (dictionary != null)
            //{
            //    foreach (var dataObject in dictionary)
            //    {
            //        var tempName = dataObject.Key;
            //        var tempDName = dataObject.Value;
            //        if (tempName == null || tempDName == null)
            //            continue;
            //        var propertyName = tempName;
            //        var propertyValue = tempDName;
            //        if (!string.IsNullOrEmpty(propertyName) && !string.IsNullOrEmpty(propertyValue))
            //        {
            //            if (propertyName.StartsWith(resourcePrefix))
            //            {
            //                var resourceKey = propertyName.Substring(resourcePrefix.Length);
            //                if (stringDictionary.ContainsKey(resourceKey))
            //                {
            //                    stringDictionary[resourceKey].Localized = propertyValue;
            //                }
            //            }
            //        }
            //    }
            //    SendChanges();
            //}
            foreach (var keyValuePair in stringDictionary)
            {
                keyValuePair.Value.Localized = null;
            }
            SendChanges();
        }


        //This part need for refresh strings in ViewModel after Concatenation or if in viewModel used casted to System.String value(that don't support refresh scenario)
        #region WeakReferencePart
        /// <summary>
        /// Ping all ViewModels for refresh values
        /// </summary>
        private void SendChanges()
        {
            WeakReference[] weakArray;
            lock (weakReferences)
            {
                weakArray = weakReferences.ToArray();
                foreach (var weakLink in weakArray)
                {
                    var target = weakLink.Target as IListenStringResourceProvider;
                    if (target != null)
                    {
                        target.OnPopulateStringResourceProvider();
                    }
                    else
                    {
                        weakReferences.Remove(weakLink);
                    }
                }
            }
        }

        private List<WeakReference> weakReferences = new List<WeakReference>();
        /// <summary>
        /// Start weak listen for refresh string resources
        /// </summary>
        /// <param name="listenObj"></param>
        public void AddWeakHandler(IListenStringResourceProvider listenObj)
        {
            lock (weakReferences)
            {
                CleanRefference();
                foreach (var weakReference in weakReferences)
                {
                    if (weakReference.IsAlive && weakReference.Target == listenObj)
                    {
                        return;
                    }
                }
                weakReferences.Add(new WeakReference(listenObj));
            }
        }

        /// <summary>
        /// delete died refference
        /// </summary>
        private void CleanRefference()
        {
            lock (weakReferences)
            {
                var weakArray = weakReferences.ToArray();
                foreach (var weakLink in weakArray)
                {
                    if (!weakLink.IsAlive)
                    {
                        weakReferences.Remove(weakLink);
                    }
                }
            }
        }
        #endregion


        /// <summary>
        /// Default values of string
        /// </summary>
        private static void SetDefaultValues()
        {
            //common
            AddlocalizedString(Keys.UiSettings, "UI Settings");
            AddlocalizedString(Keys.Intensity, "Intensity");
            AddlocalizedString(Keys.Wavelength, "Wavelength");
            AddlocalizedString(Keys.ExceptionInfo, "Exception Info");
            AddlocalizedString(Keys.CopyToClipboard, "Copy");
            AddlocalizedString(Keys.Close, "Close");
            AddlocalizedString(Keys.LogViewer, "Log Viewer");
            AddlocalizedString(Keys.Save, "Save");
            AddlocalizedString(Keys.ExperimentName, "Experiment Name");
            AddlocalizedString(Keys.Category, "Category");
            AddlocalizedString(Keys.PhysicsModel, "Physics Model");
            AddlocalizedString(Keys.Spectrometer, "Spectrometer");
            AddlocalizedString(Keys.WaveRange, "Wave Range");
            AddlocalizedString(Keys.From, "From");
            AddlocalizedString(Keys.nm, "nm");
            AddlocalizedString(Keys.To, "To");
            AddlocalizedString(Keys.DistanceList, "Distance List");
            AddlocalizedString(Keys.BaseDistance, "Base Distance");
            AddlocalizedString(Keys.Cancel, "Cancel");
            AddlocalizedString(Keys.CcdLevels, "Ccd Levels");
            AddlocalizedString(Keys.SpectrometerStatus, "Spectrometer Status");
            AddlocalizedString(Keys.Offline, "Offline");
            AddlocalizedString(Keys.Online, "Online");
            AddlocalizedString(Keys.Exposure, "Exposure");
            AddlocalizedString(Keys.EnterToSaveNowIsMs, "Enter to save (now {0}ms)");
            AddlocalizedString(Keys.RemoveNoise, "Remove Noise");
            AddlocalizedString(Keys.Normalize, "Normalize");
            AddlocalizedString(Keys.Led, "Led");
            AddlocalizedString(Keys.CurrentDistance, "Current Distance");
            AddlocalizedString(Keys.GoToDistance, "Go To Distance");
            AddlocalizedString(Keys.SaveDataAs, "Save Data As");
            AddlocalizedString(Keys.CurrentL, "Current L");
            AddlocalizedString(Keys.Noise, "Noise");
            AddlocalizedString(Keys.SpectraForAllDistances, "Spectra For All Distances");
            AddlocalizedString(Keys.ShowSavedData, "Show Saved Data");
            AddlocalizedString(Keys.No, "No");
            AddlocalizedString(Keys.Yes, "Yes");
            AddlocalizedString(Keys.LogScale, "Log Scale");
            AddlocalizedString(Keys.DivideToBase, "Divide To Base");
            AddlocalizedString(Keys.Desc, "Desc");
            AddlocalizedString(Keys.NotNoise, "Not Noise");
            AddlocalizedString(Keys.Normalized, "Normalized");
            AddlocalizedString(Keys.PleaseChooseAction, "Please Choose Action");
            AddlocalizedString(Keys.ShowExperimentList, "Show Experiment List");
            AddlocalizedString(Keys.NewExperiment, "New Experiment");
            AddlocalizedString(Keys.Export, "Export");
            AddlocalizedString(Keys.Import, "Import");
            AddlocalizedString(Keys.About, "About");
            AddlocalizedString(Keys.ExperimentListIsEmpty, "Experiment List is empty");
            AddlocalizedString(Keys.AddNewExperiment, "Add New Experiment");
            AddlocalizedString(Keys.SpectraWay, "SpectraWay");
            AddlocalizedString(Keys.LastOperation, "Last Operation");
            AddlocalizedString(Keys.ExperimentDialog, "Experiment Dialog");
            AddlocalizedString(Keys.Settings, "Settings");
            AddlocalizedString(Keys.SettingsDialog, "Settings Dialog");
            AddlocalizedString(Keys.Language, "Language");
            AddlocalizedString(Keys.SavePath, "Save Path");
            AddlocalizedString(Keys.Browse, "Browse");




            AddlocalizedString(Keys.Initializing, "Initializing");
            AddlocalizedString(Keys.Saving, "Saving");
            AddlocalizedString(Keys.PleaseWait, "Please, wait");
            AddlocalizedString(Keys.AllChangesAreSaved, "All changes are saved");
            AddlocalizedString(Keys.Experiment_PLACE_WasAdded, "Experiment '{0}' was added");//"Experiment '{0}' was added"
            AddlocalizedString(Keys.AttemptToSetExposureTimeTo_PLACE_Ms, "Attempt to set Exposure Time to {0}ms");//"Attempt to set Exposure Time to {0}ms"
            AddlocalizedString(Keys.ExposureTimeWasSetTo_PLACE_Ms, "Exposure Time was set to {0}ms");//"Exposure Time was set to {0}ms"
            AddlocalizedString(Keys.AttemptSetExposureTimeTo_PLACE_MsFailedSpectrometerNotReady, "Attempt to set Exposure Time to {0}ms failed, Spectrometer is not ready");//"Attempt to set Exposure Time to {0}ms failed, Spectrometer is not ready"
            AddlocalizedString(Keys.StartMovingToDistance_PLACE_Mm, "Start moving to distance {0}mm");//"Start moving to distance {0}mm"
            AddlocalizedString(Keys.DataIsEmpty, "Data is empty");

            AddlocalizedString(Keys.PleaseRunSpectrometerAndTryToGetSpectralMeasurements, "Please run Spectrometer and try to get spectral measurements");
            AddlocalizedString(Keys.DistanceDoesNotDefined, "Distance does`t defined");
            AddlocalizedString(Keys.PleaseMoveDetectorForSomeDistance, "Please move detector for some distance");
            AddlocalizedString(Keys.Base, "Base");
            AddlocalizedString(Keys.DataAlreadyExist, "Data already exist");
            AddlocalizedString(Keys.DoYouWantToReplace, "Do You really want to replace");

            AddlocalizedString(Keys.StepperAssigned, "Stepper assigned");
            AddlocalizedString(Keys.StepperIsRestarting, "Stepper is restarting");
            AddlocalizedString(Keys.StepperIsStopping, "Stepper is stopping");
            AddlocalizedString(Keys.CurrentDistanceIs_PLACE_Mm, "Current Distance is {0}mm");//"Current Distance is {0}mm"
            AddlocalizedString(Keys.StepperFailedSeeLogForDetails, "Stepper failed, see log for details");//"Stepper failed, see log for details"
            AddlocalizedString(Keys.SpectrometerFailedSeeLogForDetails, "Spectrometer failed, see log for details");//"Spectrometer failed, see log for details"
            AddlocalizedString(Keys.SpectrometerIsRestarting, "Spectrometer is restarting");
            AddlocalizedString(Keys.SpectrometerIsStopping, "Spectrometer is stopping");
            AddlocalizedString(Keys.SpectrometerAssigned, "Spectrometer assigned");
            AddlocalizedString(Keys.ConnectToSpectrometerPleaseWait, "Attempt to connect to Spectrometer, please wait");
            AddlocalizedString(Keys.SpectraForAllDistancesSuccessfullyRecieved, "spectral data for all Distances successfully recieved");
            AddlocalizedString(Keys.RetrieveParamsPleaseWait, "Parameters are retrieving, please wait");
            



            AddlocalizedString(Keys.LotisSpectrometerCannotBeUsed, "Lotis spectrometer cannot be used");//Lotis spectrometer cannot be used
            AddlocalizedString(Keys.OrminsCcdCantBeInitialize, "Ormins CCD can`t be initialize");//Ormins CCD can`t be initialize
            AddlocalizedString(Keys.OrminsCcdHitTestFailed, "Ormins CCD HitTest failed");//Ormins CCD HitTest failed
            AddlocalizedString(Keys.OrminsCcdHasIncorrectParams, "Ormins CCD has incorrect params");//Ormins CCD has incorrect params

            AddlocalizedString(Keys.RequestedDistanceLessThanMinDistance, "Requested Distance less than min Distance");//Requested Distance less than min Distance
            AddlocalizedString(Keys.CurrentDistanceSetAsPossibleMinimalDistance_PLACE_Mm, "Current Distance set as possible minimal Distance ({0}mm)");//Current Distance set as possible minimal Distance, {0}mm



            #region Code for generating .mpx resourse strings

            //            StringBuilder sb1 = new StringBuilder();
            //            StringBuilder sb2 = new StringBuilder();
            //
            //            Instance.stringDictionary.ForEach(a =>
            //            {
            //                sb1.Append("<DisplayString ElementID=\"SpectraWay.");
            //                sb1.Append(a.Key);
            //
            //                sb1.AppendLine("\">");
            //                sb1.Append("  <Name>");
            //                sb1.Append(a.Value.Default);
            //                
            //                sb1.AppendLine("</Name>");
            //                sb1.AppendLine("</DisplayString>");
            //
            ////<StringResource ID="Microsoft.SQLServer.Visualization.Library.StringResource.DataCenterDashboard.Home"/>
            //                sb2.Append("<StringResource ID=\"Microsoft.SQLServer.Visualization.Library.StringResource.DataCenterDashboard.");
            //                sb2.Append(a.Key);
            //                sb2.AppendLine("\"/>");
            //            });
            //            var ak712 = sb1.ToString();
            //            var ak713 = sb2.ToString();
            //            var i = 0.3;

            #endregion
        }

        private static void AddlocalizedString(string key, string value)
        {
            Instance.stringDictionary[key] = new LocalizedString(key, value);
        }

        public LocalizedString GetBindableValue(string key)
        {
            LocalizedString value;
            if (stringDictionary.TryGetValue(key, out value))
            {
                return value;
            }
#if DEBUG
            throw new Exception(string.Format("Incorrect string resource - '{0}'", key));
#endif
            return null;
        }

        public LocalizedString this[string key]
        {
            get
            {
                return StringResourceProvider.Instance.GetBindableValue(key);
            }
        }
    }


    public sealed class StringResourceProviderWrapper
    {
        public StringResourceProvider Instance => StringResourceProvider.Instance;
    }

}
