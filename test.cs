using System;

namespace RealEstateSimulator
{
    public static class Utility
    {
        /// <summary>
        /// 指定された少数に丸める
        /// </summary>
        /// <param name="value">丸める値</param>
        /// <param name="count">丸める位</param>
        /// <returns></returns> <summary>
        public static float round(float value, int count)
        {
            var factor = 10f * ((float)(count - 1));
            value *= factor;

            value = (float)Math.Round((double)value);
            return value / factor;
        }
    }
    /// <summary>
    /// 建物構造の種類
    /// </summary>
    /// <remarks>
    /// <para>耐用年数...法律上の建物の寿命。耐用年数の残りから減価償却費が計算される</para>
    /// </remarks>
    public enum StructureKind
    {
        /// <summary>
        /// 未定義
        /// </summary>
        None, 

        /// <summary>
        /// 木造
        /// </summary>
        /// <remarks>
        /// 住宅用耐用年数22年
        /// </remarks>
        Wooden, 

        /// <summary>
        /// 軽量鉄骨造
        /// </summary>
        /// <remarks>
        /// 住宅用耐用年数19年
        /// </remarks>
        LightSteelStructure, 

        /// <summary>
        /// 中量鉄骨造
        /// </summary>
        /// <remarks>
        /// 住宅用耐用年数27年
        /// </remarks>
        MiddleSteelStructure, 

        /// <summary>
        /// 重量鉄骨造
        /// </summary>
        /// <remarks>
        /// 住宅用耐用年数34年
        /// </remarks>
        HeavySteelStructure, 

        /// <summary>
        /// 鉄筋コンクリート造
        /// </summary>
        /// <remarks>
        /// 住宅用耐用年数47年
        /// </remarks>
        RC, 

        /// <summary>
        /// 鉄骨鉄筋コンクリート造
        /// </summary>
        /// <remarks>
        /// 住宅用耐用年数47年
        /// </remarks>
        SRC, 
    }

    /// <summary>
    /// 土地権利の種類
    /// </summary>
    public enum LandRightKind 
    {
        /// <summary>
        /// 未定義
        /// </summary> 
        None, 

        /// <summary>
        /// 土地所有権
        /// </summary>
        LandOwnerShip, 

        /// <summary>
        /// 借地権
        /// </summary>
        LeaseHoldRight
    }

    /// <summary>
    /// 不動産に関する情報
    /// </summary>
    public class EstateInfo
    {
        #region Property

        /// <summary>
        /// 物件価格[円]
        /// </summary>
        /// <value></value>
        public int Price{ get; }

        /// <summary>
        /// 想定年間収入[円]
        /// </summary>
        /// <value></value>
        public int EstimatedAnnualIncome { get; }

        /// <summary>
        /// 想定月間収入[円]
        /// </summary>
        /// <value></value>
        public int EstimatedMonthlyIncome{ get{ return EstimatedAnnualIncome / 12; } }

        /// <summary>
        /// 表面利回り[%] =  (想定年間収入/物件価格）* 100
        /// </summary>
        /// <value></value>
        public float GrossYield 
        {
            get
            {
                if(Price <= 0)
                    return 0.0f;

                var grossYield = ((float)EstimatedAnnualIncome / (float)Price) * 100.0f;
                return (float)Math.Floor((double)(grossYield * 100.0f)) / 100.0f;
            }
        }

        /// <summary>
        /// 純利益
        /// </summary>
        /// <value></value>
        public int NetProfit { get{ return EstimatedAnnualIncome - Expenses - FixedAssetTax; } }

        /// <summary>
        /// 還元利回り[%]
        /// </summary> 
        /// <value></value>
        public float CapitalizationRate { get{ return ((float)NetProfit / (float)Price) * 100.0f; } }

        /// <summary>
        /// 所在地
        /// </summary>
        /// <value></value>
        public string Location { get; }

        /// <summary>
        /// 築年月
        /// </summary>
        /// <value></value>
        public DateTime ConstructionYear { get; }

        /// <summary>
        /// 築年数
        /// </summary>
        /// <value></value>
        public int Age 
        {
             get
             {
                if(ConstructionYear == null)
                    return 0;

                // 築年数 = 今の年 - 築年
                var currentDate = DateTime.Now;
                return (currentDate.Year - ConstructionYear.Year) + 1;
             }
        }

        /// <summary>
        /// 建物構造
        /// </summary>
        /// <value></value>
        public StructureKind StructureKind { get; }

        /// <summary>
        /// 土地権利
        /// </summary>
        /// <value></value>
        public LandRightKind LandRightKind { get; }

        /// <summary>
        /// 土地面積[m^2]
        /// </summary>
        /// <value></value>
        public float LandArea { get; }

        /// <summary>
        /// 建物面積[m^2]
        /// </summary>
        /// <value></value>
        public float BuildingArea { get; }

        /// <summary>
        /// 階数
        /// </summary>
        /// <value></value>
        public float FloorCount { get; }

        /// <summary>
        /// 総戸数
        /// </summary>
        /// <value></value>
        public int TotalUnitNumber { get; }

        /// <summary>
        /// 建蔽率[%]
        /// その土地にどれぐらいの広さの一階をとれるか
        /// 地域の属性や都市計画によって土地ごとに決められている
        /// </summary>
        /// <value></value>
        public float BuildingCoverageRatio { get; }

        /// <summary>
        /// 容積率[%]
        /// 敷地面積に対する延べ床面積の割合
        /// 都市計画によって定められている
        /// </summary>
        /// <value></value>
        public float FloorAreaRatio { get; }
         
        /// <summary>
        /// 路線価
        /// </summary> 
        /// <remarks>
        /// 1m^2あたりの実際の値段
        /// 下記のホームページから検索できる
        /// https://www.chikamap.jp/chikamap/PrefecturesSelect?mid=124
        /// </remarks>
        /// <value></value>
        public int RoadPrice { get; set; } = 0;

        /// <summary>
        /// 積算価格
        /// </summary>
        /// <value></value>
        public int EstimatedPrice { get; set; } = 0;

        /// <summary>
        /// 収益価格
        /// </summary>
        /// <remarks>
        /// 収益価格 = 純利益 / 還元利回り
        /// </remarks>
        /// <value></value>
        public int ProfitPrice { get{ return (int)((float)NetProfit / (CapitalizationRate / 100.0f)); } }

        /// <summary>
        /// 建物評価額
        /// </summary>
        /// <value></value>
        public int BuildingAppraisedValue { get; set; } = 0;

        /// <summary>
        /// 土地評価額
        /// </summary>
        /// <value></value>
        public int LandAppraisedValue { get; set; } = 0;

        /// <summary>
        /// 固定資産税
        /// </summary>
        /// <value></value>
        public int FixedAssetTax { get { return LandFixedAssetTax + BuildingFixedAssetTax; } }

        /// <summary>
        /// 土地の固定資産税
        /// </summary>
        /// <remarks>
        /// 標準税率1.4%で計算する
        /// </remarks>
        /// <value></value>
        public int LandFixedAssetTax { get{ return (int)((float)LandAppraisedValue * 0.014f); } }

        /// <summary>
        /// 建物の固定資産税
        /// </summary>
        /// <remarks>
        /// 標準税率1.4%で計算する
        /// </remarks>
        /// <value></value>
        public int BuildingFixedAssetTax { get { return (int)((float)BuildingAppraisedValue * 0.014f); } }

        /// <summary>
        /// 諸経費
        /// </summary>
        /// <value></value>
        public int Expenses { get; set; } = 0;
 
        #endregion

        #region Method

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="price">物件価格</param>
        /// <param name="estimatedAnnualIncome">想定年間収入</param>
        /// <param name="location">所在地</param>
        /// <param name="constructionYear">築年月</param>
        /// <param name="structureKind">建物構造の種類</param>
        /// <param name="landRightKind">土地権利の種類</param>
        /// <param name="landArea">土地面積</param>
        /// <param name="buildingArea">建物面積</param>
        /// <param name="floorCount">階数</param>
        /// <param name="totalUnitNumber">総戸数</param>
        /// <param name="buildingCoverageRatio">建蔽率</param>
        /// <param name="floorAreaRatio">容積率</param>
        public EstateInfo(
            int price, 
            int estimatedAnnualIncome, 
            string location, 
            DateTime constructionYear, 
            StructureKind structureKind,
            LandRightKind landRightKind,
            float landArea, 
            float buildingArea,
            int floorCount, 
            int totalUnitNumber, 
            float buildingCoverageRatio, 
            float floorAreaRatio
            )
        {
            Price = price;
            EstimatedAnnualIncome = estimatedAnnualIncome;
            Location = location;
            ConstructionYear = constructionYear;
            StructureKind = structureKind;
            LandRightKind = landRightKind;
            LandArea = landArea;
            BuildingArea = buildingArea;
            FloorCount = floorCount;
            TotalUnitNumber = totalUnitNumber;
            BuildingCoverageRatio = buildingCoverageRatio;
            FloorAreaRatio = floorAreaRatio;
        }

        /// <summary>
        /// 積算価格をシミュレーションする
        /// </summary>
        /// <remarks>
        /// 路線価を設定した後に処理される必要がある
        /// </remarks>
        public void simulateEstimatedPrice()
        {
            // todo: https://www.toushi-hakase.com/simulation/ をスクレイピングして積算価格を計算する
            BuildingAppraisedValue = 1800_0000;
            LandAppraisedValue = 2665_0000;
            EstimatedPrice = 4465_0000;
        }

        /// <summary>
        /// 情報をログ出力する
        /// </summary>
        public void log()
        {
            Console.WriteLine("物件価格 " + Price + "円");
            Console.WriteLine("想定年間収入 " + EstimatedAnnualIncome + "円");
            Console.WriteLine("想定月間収入 " + EstimatedMonthlyIncome+ "円");
            Console.WriteLine("表面利回り " + GrossYield + "%");
            Console.WriteLine("土地評価額 " + LandAppraisedValue + "円");
            Console.WriteLine("建物評価額 " + BuildingAppraisedValue + "円");
            Console.WriteLine("積算価格 " + EstimatedPrice + "円(" + (((float)EstimatedPrice / (float)Price) * 100.0f)+ "%)");
            Console.WriteLine("収益価格 " + ProfitPrice + "円(" + (((float)ProfitPrice / (float)Price) * 100.0f) + "%)");
            Console.WriteLine("純利益 " + NetProfit + "円");
            Console.WriteLine("還元利回り " + CapitalizationRate + "%");
            Console.WriteLine("固定資産税 " + FixedAssetTax + "円");
            Console.WriteLine("所在地 " + Location);
            Console.WriteLine("築年 " + ConstructionYear.Year + "年 (築" + Age + "年)");
            Console.WriteLine("建物構造 " + StructureKind);
            Console.WriteLine("土地権利 " + LandRightKind);
            Console.WriteLine("土地面積 " + LandArea + "m^2");
            Console.WriteLine("建物面積 " + BuildingArea + "m^2");
            Console.WriteLine(FloorCount + "階建て");
            Console.WriteLine("総戸数 " + TotalUnitNumber + "戸");
            Console.WriteLine("建蔽率 " + BuildingCoverageRatio + "%");
            Console.WriteLine("容積率 " + FloorAreaRatio + "%");
        }

        #endregion
    }

    /// <summary>
    /// ローンに関する情報
    /// </summary>
    public class RoanInfo 
    {
        #region Property

        /// <summary>
        /// 融資金額[円]
        /// </summary>
        /// <value></value>
        public int Amount { get; }

        /// <summary>
        /// 金利[%]
        /// </summary>
        /// <value></value>
        public float InterestRate { get; }
        
        /// <summary>
        /// 期間[年]
        /// </summary>
        /// <value></value>
        public int Period { get; }

        /// <summary>
        /// 総返済回数
        /// </summary>
        /// <value></value>
        public int TotalRepaymentCount{ get { return Period * 12; } }

        #endregion

        #region Method

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="amount">融資金額</param>
        /// <param name="interactRate">金利[%]</param>
        /// <param name="period">期間</param>
        public RoanInfo(int amount, float interactRate, int period)
        {
            Amount = amount;
            InterestRate = interactRate;
            Period = period;
        }

        /// <summary>
        /// 元利均等返済で月の返済額を計算する
        /// </summary>
        /// <remarks>
        /// 元利均等返済...毎月の返済額が均等になるような返済方法 
        /// メリット...初期の返済額が大きくならない
        /// デメリット...元金の返済スピードが遅いため総支払額は大きくなる
        /// /// </remark>
        /// <returns></returns>
        public int calcRepaymentByEqualPrincipalAndInterestRepayment()
        {
            var monthlyInteractRate = (InterestRate / 100.0f) / 12.0f;
           
            var numerator = (float)Amount * monthlyInteractRate * (Math.Pow(1.0f + monthlyInteractRate, TotalRepaymentCount));
            var denominator = (Math.Pow(1.0f + monthlyInteractRate, TotalRepaymentCount)) - 1.0f;
           
            return (int)(numerator / denominator);
        }

        /// <summary>
        /// 元金均等返済で月の返済額を計算する
        /// </summary>
        /// <remarks>
        /// 元金均等返済...毎月の返済額が均等になるような返済方法 
        /// メリット...元金の返済スピードが速く、総支払額を抑えることができる
        /// デメリット...初期の支払額が高額になる
        /// </remark>
        /// <returns></returns>
        public int calcRepaymentByEqualPrincipalRepayment(int elapsedRepaymentCount)
        {
            var monthlyInteractRate = (InterestRate / 100.0f) / 12;
            var remainAmount = Amount - (Amount * monthlyInteractRate * elapsedRepaymentCount);

            var repayment = remainAmount / TotalRepaymentCount + (int)((float)remainAmount * monthlyInteractRate);
            return (int)repayment;
        }

        /// <summary>
        /// 情報をログ出力する
        /// </summary>
        public void log()
        {
            Console.WriteLine("借入金額 " + Amount + "円");
            Console.WriteLine("金利 " + InterestRate + "%");
            Console.WriteLine("期間 " + Period + "年");

            var monthlyRepayment = calcRepaymentByEqualPrincipalAndInterestRepayment();
            Console.WriteLine("月額借入支払: " + monthlyRepayment + "円");
        }

        #endregion
    }
   
    /// <summary>
    /// シミュレーション情報
    /// </summary>
    public class SimulationInfo 
    {
        #region Property

        /// <summary>
        /// 不動産情報
        /// </summary>
        /// <value></value>
        public EstateInfo EstateInfo { get; }

        /// <summary>
        /// ローン情報
        /// </summary>
        /// <value></value>
        public RoanInfo RoanInfo { get; }

        /// <summary>
        /// 空室率を考慮した想定年間収入
        /// </summary>
        /// <returns></returns>
        public int EstimatedAnnualIncome { get { return EstateInfo != null ? (int)((float)EstateInfo.EstimatedAnnualIncome * FullOccupancyRate) : 0;} }

        /// <summary>
        /// 満室率[%]
        /// </summary> 
        /// <remarks>
        /// 築20年までは90%, それ以後は80%で見ておくと望ましい
        /// </remarks>
        /// <value></value>
        public float FullOccupancyRate { get; set; } = 90.0f;

        /// <summary>
        /// 返済比率[%]
        /// </summary>
        /// <remarks>
        /// 50%以下が望ましい
        /// </remarks> 
        /// <value></value>
        public float RepaymentRatio 
        {
            get
            {
                if(EstateInfo == null || RoanInfo == null)
                    return 0.0f;

                var monthlyRepayment = RoanInfo.calcRepaymentByEqualPrincipalAndInterestRepayment();
                return ((float)monthlyRepayment / (float)EstateInfo.EstimatedMonthlyIncome) * 100.0f;
            }
        }
        #endregion

        #region Method

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="estateInfo">不動産情報</param>
        /// <param name="roanInfo">ローン情報</param>
        public SimulationInfo(EstateInfo estateInfo, RoanInfo roanInfo)
        {
            EstateInfo = estateInfo;
            RoanInfo = roanInfo;
        }

        /// <summary>
        /// 情報をログ出力する
        /// </summary>
        public void log()
        {
            if(EstateInfo != null)
                EstateInfo.log();

            if(RoanInfo != null)
                RoanInfo.log();

            // 返済比率は
          
            Console.WriteLine("返済比率: " + RepaymentRatio + "%");
        }

        #endregion
    }

    public class Program
    {
        public static void Main()
        {
            // 物件価格
            var price = 8842_0000;

            // 想定年間収入
            var estimatedAnnualIncome = 42_0000 * 12;

            // 所在地
            var location = "大阪府大阪市福島区大開2丁目";

            // 築年月
            var constructionYear = new DateTime(1998, 6, 1);

            // 建物構造
            var structureKind = StructureKind.Wooden;

            // 土地権利
            var landRight = LandRightKind.LandOwnerShip;

            // 土地面積
            var landArea = 130.0f;

            // 建物面積
            var buildingArea = 120.0f;

            // 階数
            var floorCount = 3;

            // 総戸数
            var totalUnitNumber = 6;

            // 建蔽率
            var buildingCoverageRatio = 60.0f;

            // 容積率
            var floorAreaRatio = 200.0f;

            var estateInfo = new EstateInfo(
                price, 
                estimatedAnnualIncome,
                location, 
                constructionYear, 
                structureKind, 
                landRight, 
                landArea, 
                buildingArea,
                floorCount, 
                totalUnitNumber, 
                buildingCoverageRatio, 
                floorAreaRatio
                );
          
            // 路線価
            estateInfo.RoadPrice = 20_5000;
            
            // 積算価格をシミュレーション
            estateInfo.simulateEstimatedPrice();

            // 諸経費
            estateInfo.Expenses = 35000;

            // todo: ワーストの金利で計算できるようにする（危険ゾーンを明記する）
            var roanInfo = new RoanInfo(8570_0000, 2.55f, 35);

            var simurationInfo = new SimulationInfo(estateInfo, roanInfo);           
            simurationInfo.log();
           
            // todo: 原状回復費
            // todo: 大規模修繕費
            // todo: 不動産所得税
            // todo: 家賃下落率や固定資産税などを計算して、実利回りを計算できるようにする
            // todo: イールドギャップを計算するようにする
            // 35年先までの毎年のキャッシュフローを計算できるようにする
        }
    }
}