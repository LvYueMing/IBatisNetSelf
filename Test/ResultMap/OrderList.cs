using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultMap
{
    public class OrderList
    {
        /// <summary>
        /// 医嘱ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 标志
        /// </summary>
        public string Flag { get; set; }

        /// <summary>
        /// 医嘱状态
        /// </summary>
        public string OrderStatus { get; set; }

        /// <summary>
        /// 执行状态
        /// </summary>
        public string ExecuteStatus { get; set; }

        /// <summary>
        /// 申请单状态
        /// </summary>
        public string ApplyStatus { get; set; }

        /// <summary>
        /// 申请单编号
        /// </summary>
        public string ApplyNo { get; set; }

        /// <summary>
        /// 开始时间（短格式）
        /// </summary>
        public string StartShortDate { get; set; }

        /// <summary>
        /// 医嘱内容
        /// </summary>
        public string OrderContent { get; set; }

        /// <summary>
        /// 皮试信息
        /// </summary>
        public string SkinTestInfo { get; set; }

        /// <summary>
        /// 剂型和规格
        /// </summary>
        public string PackageDescription { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 剂型
        /// </summary>
        public string DosageForm { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 每天用量
        /// </summary>
        public string DailyDosage { get; set; }

        /// <summary>
        /// 本次数量
        /// </summary>
        public string ThisDays { get; set; }

        /// <summary>
        /// 已执行日期
        /// </summary>
        public string ExecutedDate { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 录入员
        /// </summary>
        public string InputWorker { get; set; }

        /// <summary>
        /// 录入时间
        /// </summary>
        public string InputDate { get; set; }

        /// <summary>
        /// 入账时间
        /// </summary>
        public string AccountDate { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        public string ExecuteDate { get; set; }

        /// <summary>
        /// 取药部门
        /// </summary>
        public string MedicineDept { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// 长期标志
        /// </summary>
        public string LongFlag { get; set; }

        /// <summary>
        /// 开始日期（长格式）
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// 父ID
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 医嘱类型标识
        /// </summary>
        public string TypeFlag { get; set; }

        /// <summary>
        /// 开单医生ID
        /// </summary>
        public string StartWorkerId { get; set; }

        /// <summary>
        /// 备注文本
        /// </summary>
        public string CommentText { get; set; }

        /// <summary>
        /// 状态标识
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 子序号
        /// </summary>
        public int Subseq { get; set; }

        /// <summary>
        /// 药品抗菌药物级别
        /// </summary>
        public string Attr { get; set; }

        /// <summary>
        /// 医生用药权限
        /// </summary>
        public string CanUse { get; set; }

        /// <summary>
        /// 医嘱类型（药品/杂费/文本）
        /// </summary>
        public string OrderType { get; set; }

        /// <summary>
        /// 光照标志
        /// </summary>
        public string LightFlag { get; set; }

        /// <summary>
        /// 材料ID
        /// </summary>
        public string MaterialId { get; set; }

        /// <summary>
        /// 杂费ID
        /// </summary>
        public string SundryFeeId { get; set; }

        /// <summary>
        /// 文本内容
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 材料名称（含医保信息）
        /// </summary>
        public string MaterialName { get; set; }

        /// <summary>
        /// 停止时间
        /// </summary>
        public string StopDate { get; set; }

        /// <summary>
        /// 停止员
        /// </summary>
        public string StopWorker { get; set; }

        /// <summary>
        /// 用法
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 材料ID与包装ID组合
        /// </summary>
        public string PassMaterialId { get; set; }

        /// <summary>
        /// 小计金额
        /// </summary>
        public decimal Subtotal { get; set; }

        /// <summary>
        /// 皮试结果1
        /// </summary>
        public string SkinTestResult1 { get; set; }

        /// <summary>
        /// 皮试员1
        /// </summary>
        public string SkinTester1 { get; set; }

        /// <summary>
        /// 皮试员2
        /// </summary>
        public string SkinTester2 { get; set; }

        /// <summary>
        /// 材料部门ID
        /// </summary>
        public string MaterialDeptId { get; set; }

        /// <summary>
        /// 包装ID
        /// </summary>
        public string PackageId { get; set; }

        /// <summary>
        /// 皮试结果
        /// </summary>
        public string SkinTestResult { get; set; }

        /// <summary>
        /// 首次执行次数
        /// </summary>
        public int FirstExecutions { get; set; }

        /// <summary>
        /// 末次执行次数
        /// </summary>
        public int LastExecutions { get; set; }

        /// <summary>
        /// 首末执行次数
        /// </summary>
        public string FirstLastExecutions { get; set; }

        /// <summary>
        /// 首次执行数量
        /// </summary>
        public int FirstExecAmount { get; set; }

        /// <summary>
        /// 末次执行数量
        /// </summary>
        public int LastExecAmount { get; set; }

        /// <summary>
        /// 剂量
        /// </summary>
        public string Dosage { get; set; }

        /// <summary>
        /// 杂费名称
        /// </summary>
        public string SundryFeeName { get; set; }

        /// <summary>
        /// 药品名称
        /// </summary>
        public string DrugName { get; set; }

        /// <summary>
        /// 样本类型
        /// </summary>
        public string SampleType { get; set; }

        /// <summary>
        /// 组合标志
        /// </summary>
        public string ComboFlag { get; set; }

        /// <summary>
        /// 处方类型
        /// </summary>
        public string PrescriptionType { get; set; }

        /// <summary>
        /// 医嘱名称
        /// </summary>
        public string OrderName { get; set; }

        /// <summary>
        /// 展开标志
        /// </summary>
        public string Expand { get; set; }

        /// <summary>
        /// 签名信息
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// 国家医保码
        /// </summary>
        public string NationalInsuranceCode { get; set; }

        /// <summary>
        /// 国家医保项目名称
        /// </summary>
        public string NationalInsuranceName { get; set; }

        /// <summary>
        /// 送检物
        /// </summary>
        public string InspectItem { get; set; }

        /// <summary>
        /// 病历文件ID
        /// </summary>
        public string MrFileId { get; set; }

        /// <summary>
        /// 病历文档类型
        /// </summary>
        public string MrDocClass { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 录入方式
        /// </summary>
        public string InputWay { get; set; }

        /// <summary>
        /// 周期标志
        /// </summary>
        public string CycleMarker { get; set; }

        /// <summary>
        /// 确认时间
        /// </summary>
        public string AckDate { get; set; }

        /// <summary>
        /// 执行人
        /// </summary>
        public string Executor { get; set; }

        /// <summary>
        /// 频率字典
        /// </summary>
        public string FrequenceDict { get; set; }

        /// <summary>
        /// 跨院医嘱URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 计划停止时间
        /// </summary>
        public string PlannedStopDate { get; set; }

        /// <summary>
        /// 已执行/未执行天数
        /// </summary>
        public string ExecutedDaysInfo { get; set; }

        /// <summary>
        /// 用药原因
        /// </summary>
        public string UseReason { get; set; }

    }
}
