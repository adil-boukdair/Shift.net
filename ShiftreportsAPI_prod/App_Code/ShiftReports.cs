using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

public class ShiftReports
{

    public static string DecodeChecklist(IEnumerable<dynamic> list)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var item in list)
        {
            sb.Append(item.q_text + "-" + item.q_uuid + "-" + item.q_category + "-" + item.shift_num);
            sb.Append("/");
        }
       
        return sb.ToString();
    }

    public static string FormatShiftTime(string time)
    {
        string h = "";
        string m= "";
        if(time.Length>1)
        {
            h = time.Substring(0, 2);
            if(time.Length==4)
            {
                m = time.Substring(2, 2);
            }
            else
            {
                m = "00";
            }           
        }
        else
        {
            h = "0000";
        }

        if (h.Substring(1,1)=="0")
        {
            h = h.Substring(0, 1);
        }

           
            int hh  = Int32.Parse(h);
            int mm = Int32.Parse(m);
            TimeSpan span = new TimeSpan(hh, mm, 00);
            DateTime shiftTime = DateTime.Today.Add(span);
 
        return shiftTime.ToString("hh:mm tt");
    }


    #region ManagerID //login
    private static string loginID;
    public static string LoginID
    {
        get { return loginID; }
        set { loginID = value; }
    }
    #endregion

    #region firtsTime //login
    private static bool hasStore;
    public static bool HasStore
    {
        get { return hasStore; }
        set { hasStore = value; }
    }

    private static bool loaded;
    public static bool Loaded
    {
        get { return loaded; }
        set { loaded = value; }
    }

    private static bool newLoaded;
    public static bool NewLoaded
    {
        get { return newLoaded; }
        set { newLoaded = value; }
    }
    #endregion

    private static string selectedShiftNumber;
    public static string SelectedShiftNumber
    {
        get { return selectedShiftNumber; }
        set { selectedShiftNumber = value; }

    }

    private static string customStoreName;
    public static string CustomStoreName
    {
        get { return customStoreName; }
        set { customStoreName = value; }

    }

    public static List<int> RackIDs;
    public static List<int> CashierIDs;

    #region Store
    private static bool storeOK;
    public static bool StoreOK
    {
        get { return storeOK; }
        set { storeOK = value; }
    }


    private static int storeID;
    public static int StoreID
    {
        get { return storeID; }
        set { storeID = value; }
    }


    private static string storeName;
    public static string StoreName
    {
        get { return storeName; }
        set { storeName = value; }
    }

    private static string storeCity;
    public static string StoreCity
    {
        get { return storeCity; }
        set { storeCity = value; }
    }

    private static string storeAddress1;
    public static string StoreAddress1
    {
        get { return storeAddress1; }
        set { storeAddress1 = value; }
    }

    private static string storeAddress2;
    public static string StoreAddress2
    {
        get { return storeAddress2; }
        set { storeAddress2 = value; }
    }

    private static string storeState;
    public static string StoreState
    {
        get { return storeState; }
        set { storeState = value; }
    }

    private static string storeZip;
    public static string StoreZip
    {
        get { return storeZip; }
        set { storeZip = value; }
    }

    private static string shift1Begin;
    public static string Shift1Begin
    {
        get { return shift1Begin; }
        set { shift1Begin = value; }
    }

    private static string shift1End;
    public static string Shift1End
    {
        get { return shift1End; }
        set { shift1End = value; }
    }

    private static string shift2Begin;
    public static string Shift2Begin
    {
        get { return shift2Begin; }
        set { shift2Begin = value; }
    }

    private static string shift2End;
    public static string Shift2End
    {
        get { return shift2End; }
        set { shift2End = value; }
    }

    private static string shift3Begin;
    public static string Shift3Begin
    {
        get { return shift3Begin; }
        set { shift3Begin = value; }
    }

    private static string shift3End;
    public static string Shift3End
    {
        get { return shift3End; }
        set { shift3End = value; }
    }


    #endregion

    #region Cashier
    private static bool cashiersOK;
    public static bool CashiersOK
    {
        get { return cashiersOK; }
        set { cashiersOK = value; }
    }

    private static string cashierName;
    public static string CashierName
    {
        get { return cashierName; }
        set { cashierName = value; }
    }

    private static string cashierNumber;
    public static string CashierNumber
    {
        get { return cashierNumber; }
        set { cashierNumber = value; }
    }

    private static string cashierPassword;
    public static string CashierPassword
    {
        get { return cashierPassword; }
        set { cashierPassword = value; }
    }
    #endregion

    #region Manager
    private static bool managersOK;
    public static bool ManagersOK
    {
        get { return managersOK; }
        set { managersOK = value; }
    }

    private static string managerID;
    public static string ManagerID
    {
        get { return managerID; }
        set { managerID = value; }
    }

    private static string managerName;
    public static string ManagerName
    {
        get { return managerName; }
        set { managerName = value; }
    }

    private static string managerNumber;
    public static string ManagerNumber
    {
        get { return managerNumber; }
        set { managerNumber = value; }
    }

    private static string managerPassword;
    public static string ManagerPassword
    {
        get { return managerPassword; }
        set { managerPassword = value; }
    }
    #endregion

    #region Cigarettes
    private static bool cigsOK;
    public static bool CigsOK
    {
        get { return cigsOK; }
        set { cigsOK = value; }
    }

    private static int rackNumber;
    public static int RackNumber
    {
        get { return rackNumber; }
        set { rackNumber = value; }
    }

    private static int lottoRackNumber;
    public static int LottoRackNumber
    {
        get { return lottoRackNumber; }
        set { lottoRackNumber = value; }
    }

    private static string cigNumRack;
    public static string CigNumRack
    {
        get { return cigNumRack; }
        set { cigNumRack = value; }
    }


    private static string cigRackNr;
    public static string CigRackNr
    {
        get { return cigRackNr; }
        set { cigRackNr = value; }
    }

    private static string cigRackRows;
    public static string CigRackRows
    {
        get { return cigRackRows; }
        set { cigRackRows = value; }
    }

    private static string cigRackColumns;
    public static string CigRackColumns
    {
        get { return cigRackColumns; }
        set { cigRackColumns = value; }
    }

    private static string cigRackName;
    public static string CigRackName
    {
        get { return cigRackName; }
        set { cigRackName = value; }
    }
    #endregion

    #region Lotto
    private static bool lottosOK;
    public static bool LottosOK
    {
        get { return lottosOK; }
        set { lottosOK = value; }
    }


    private static string lottoRackNr;
    public static string LottoRackNr
    {
        get { return lottoRackNr; }
        set { lottoRackNr = value; }
    }

    private static string lottoRackRows;
    public static string LottoRackRows
    {
        get { return lottoRackRows; }
        set { lottoRackRows = value; }
    }

    private static string lottoRackColumns;
    public static string LottoRackColumns
    {
        get { return lottoRackColumns; }
        set { lottoRackColumns = value; }
    }

    private static string lottoRackName;
    public static string LottoRackName
    {
        get { return lottoRackName; }
        set { lottoRackName = value; }
    }

    #endregion

    #region Card
    public class Card
    {
        private static int id;
        public static int ID
        {
            get { return id; }
            set { id = value; }
        }


        private static string _firstName;
        public static string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        private static string lastName;
        public static string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        private static string email;
        public static string Email
        {
            get { return email; }
            set { email = value; }
        }

        private static string state;
        public static string State
        {
            get { return state; }
            set { state = value; }
        }

        private static string zip;
        public static string Zip
        {
            get { return zip; }
            set { zip = value; }
        }

        private static string month;
        public static string Month
        {
            get { return month; }
            set { month = value; }
        }

        private static string year;
        public static string Year
        {
            get { return year; }
            set { year = value; }
        }

        private static string number;
        public static string Number
        {
            get { return number; }
            set { number = value; }
        }

        private static string code;
        public static string Code
        {
            get { return code; }
            set { code = value; }
        }
    }
    #endregion

    #region Manager
    public class Manager
    {
        public Manager()
        {

        }

        public string ID;
        public string Name;
        public string Password;
        public string Email;
        public string Phone;
        public string Level;
    }
    #endregion

    #region Cashier
    public class Cashier
    {
        public Cashier()
        {

        }

        public string ID;
        public string Name;
        public string Password;
    }

    #endregion

    #region Rack
    public class Rack
    {
        public Rack()
        {

        }

        public string ID;
        public string ImgPath;
        public string RackNum;
        public string Name;
        public string NrTrays;
        public string NrColumns;
    }
    #endregion

    #region Shift
    public class Shift
    {
        public Shift()
        {

        }

        public string ID;
        public string number;
        public string beginTime;
        public string endTime;
 
    }
    #endregion

    #region TabJS
    private static bool nowThis = false;
    public static bool NowThis { get { return nowThis; } }
    #endregion

    #region Error
    private static bool isOK;
    public static bool IsOK
    {
        get { return isOK; }
        set { isOK = value; }
    }
    #endregion

    #region File
    private static bool _uploaded;
    public static bool CigsUploaded
    {
        get { return _uploaded; ; }
        set { _uploaded = value; }
    }

    private static bool _lotto_uploaded;
    public static bool LottoUploaded
    {
        get { return _lotto_uploaded; ; }
        set { _lotto_uploaded = value; }
    }

    private static bool _isNewCigs;
    public static bool IsNewCigs
    {
        get { return _isNewCigs; ; }
        set { _isNewCigs = value; }
    }

    private static bool _isNewLotto;
    public static bool IsNewLotto
    {
        get { return _isNewLotto; ; }
        set { _isNewLotto = value; }
    }

    private static string _path;
    public static string Path
    {
        get { return _path; ; }
        set { _path = value; }
    }
    #endregion

    #region ChecklistItem
    public class ChecklistItem
    {
        public ChecklistItem()
        {

        }

        public string Text;
        public string quuid;
        public string category; 
        public string default_answer;
        public string shiftNo;
        public int q_cat; // for questions_mst
    }
    #endregion

    private static string _cigsUp;
    public static string CigsUp
    {
        get { return _cigsUp; }
        set { _cigsUp = value; }
    }

    private static bool _isdeleted;
    public static bool isDeleted
    {
        get { return _isdeleted; }
        set { _isdeleted = value; }
    }
    private static bool _isComplete;
    public static bool IsComplete
    {
        get { return _isComplete; }
        set { _isComplete = value; }
    }


    private static bool _wizcomplete;
    public static bool WizComplete
    {
        get { return _wizcomplete; }
        set { _wizcomplete = value; }
    }

    private static bool _inProgress;
    public static bool InProgress
    {
        get { return _inProgress; }
        set { _inProgress = value; }
    }

    private static int _counter;
    public static int Counter
    {
        get { return _counter; }
        set { _counter = value; }
    }

    private static string _isnew;
    public static string isNewStore
    {
        get { return _isnew; }
        set { _isnew = value; }
    }


    private static string _realsrc;
    public static string RealSrc
    {
        get { return _realsrc; }
        set { _realsrc = value; }
    }

    private static bool _nowCigs;
    public static bool NowCigs
    {
        get { return _nowCigs; }
        set { _nowCigs = value; }
    }

    private static bool _nowLotto;
    public static bool NowLotto
    {
        get { return _nowLotto; }
        set { _nowLotto = value; }
    }

    private static string maxrackID;
    public static string MaxRackID
    {
        get { return maxrackID; }
        set { maxrackID = value; }
    }

    private static string maxlottorackID;
    public static string MaxLottoRackID
    {
        get { return maxlottorackID; }
        set { maxlottorackID = value; }
    }


    private static string maxshiftID;
    public static string MaxShiftID
    {
        get { return maxshiftID; }
        set { maxshiftID = value; }
    }

    private static int rack_count;
    public static int CurrentRackNumber
    {
        get { return rack_count; }
        set { rack_count = value; }
    }

    private static int _num;
    public static int Num
    {
        get { return _num; }
        set { _num = value; }
    }
    private static int rackID;
    public static int RackID
    {
        get { return rackID; }
        set { rackID = value; }
    }

    private static int loggedManagerID;
    public static int LoggedManagerID
    {
        get { return loggedManagerID; }
        set { loggedManagerID = value; }
    }

    private static string customStoreID;
    public static string CustomStoreID
    {
        get { return customStoreID; }
        set { customStoreID = value; }
    }

    private static string wizCorpID;
    public static string WizCorpID
    {
        get { return wizCorpID; }
        set { wizCorpID = value; }
    }

    private static bool yes;
    public static bool Yes
    {
        get { return yes; }
        set { yes = value; }
    }


    private static string delRackID;
    public static string DelRackID
    {
        get { return delRackID; }
        set { delRackID = value; }
    }

    private static string storeIDChecklist;
    public static string StoreIDChecklist
    {
        get { return storeIDChecklist; }
        set { storeIDChecklist = value; }
    }

    private static string quuid;
    public static string Quuid
    {
        get { return quuid; }
        set { quuid = value; }
    }

    private static bool checklistOK;
    public static bool ChecklistOK
    {
        get { return checklistOK; }
        set { checklistOK = value; }
    }

    private static string accessManagerID;
    public static string AccessManagerID
    {
        get { return accessManagerID; }
        set { accessManagerID = value; }
    }

    public class Access
    {
        public Access()
        {

        }

        public string ID;
        public string Level;
    }

    public class ManagerAccess
    {
        public ManagerAccess()
        {

        }

        public int ManagerID;
        public string Level;
    }


}