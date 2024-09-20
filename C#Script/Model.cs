#define COMPROMISE

//#define PRIMORDIAL

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Framework.Json;
using System;
using System.IO;
using Live2D.Cubism.Rendering;
using Live2D.Cubism;
using Live2D.Cubism.Framework;
using Live2D.Cubism.Framework.LookAt;
using Unity.VisualScripting;
using Live2D.Cubism.Framework.Pose;
using Live2D.Cubism.Framework.Expression;
using Live2D.Cubism.Framework.MotionFade;
using Live2D.Cubism.Framework.MouthMovement;
using Live2D.Cubism.Core;
using System.Data.SqlTypes;
using System.Drawing;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Linq;
using Live2D.Cubism.Framework.Physics;
using Live2D.Cubism.Framework.Motion;
using Live2D.Cubism.Samples.OriginalWorkflow.Expression;
using System.Reflection;
using static ExpressionItem;

public class Model : MonoBehaviour
{
    /// <summary>
    /// 成员属性
    /// </summary>
    
    //Json File
    private CubismModel3Json Live2dModel3Json = null;
    //Model Object
    private CubismModel Live2dCubismModel = null;
    //GameObject
    private GameObject Live2dObject = null;
    //Parameters Object
    private GameObject Live2dParameters = null;
    //Parts Object
    private GameObject Live2dParts = null;
    //Drawables Object
    private GameObject Live2dDrawables = null;
    //Param Items
    private Transform TransformParamAngleX=null;
    private Transform TransformParamAngleY = null;
    private Transform TransformParamAngleZ = null;
    private Transform TransformParamBodyAngleX = null;  
    private Transform TransformParamBodyAngleY = null;
    private Transform TransformParamBodyAngleZ = null;  
    private Transform TransformParamEyeBallX = null;
    private Transform TransformParamEyeBallY = null;
    //Open Models
    private static List<GameObject> OpeningLive2dModelList = new List<GameObject>();
    //Params Dictionary
    private static Dictionary<string, float> ParamItemsDic = new Dictionary<string, float>();
    //Parts List
    private static List<Tuple<string, float>> PartItemsList = new List<Tuple<string, float>>();
    //Draws List
    private static List<Tuple<string, float>> DrawItemsList = new List<Tuple<string, float>>();
    //Animation Dictionary
    private static Dictionary<int, AnimationClip> AnimItemsDic=new Dictionary<int, AnimationClip>();
#if PRIMORDIAL
    /*在官方的 Expression 表情播放存在叠加且无法恢复的问题于是弃用*/
    //Expression Index
    private static int ExpressionIndex = -1;
#else
    /*改用自定义的 Expression 表情播放状态列表*/
    //Compromise Expression List
    private static Dictionary<int, CubismExpressionData> ExpItemsDic = new Dictionary<int, CubismExpressionData>();
#endif

    //public information
    //GameObject Transform
    public static Transform Live2dObjectTransform = null;

    void Awake()
    {
        InitModel();
    }
    public void InitModel()
    {
        //删除上一个模型
        DestroyModel();
        //初始化模型
        string path = FileOperate.GetModelJsonPath();
        InitModelByPath(path);
    }

    /// <summary>
    /// 通过Path初始化模型
    /// </summary>
    private void InitModelByPath(string path)
    {
        Debug.Log("json path :" + path);
        Config.ModelPath = path;
        string filePath = Config.ModelPath;
        int index = filePath.Length - 1;
        for (; index >= 0; --index)
            if (filePath[index] == (char)'\\' || filePath[index] == (char)'/')
                break;
        Config.DirectoryPath = filePath.Substring(0, index + 1);

        try
        {
            //通过Json初始化模型
            InitJsonFunction();
            //脚本及参数初始化
            InitModelFunction();
            //生成模型的三种控件列表
            InitModelHarmonic();
            //生成模型的表情列表
            InitModelExpression();
            //根据谐波控件列表初始化控件参数
            InitSelfModelParameters();
            InitSelfModelParts();
            InitSelfModelDrawables();

        }
        catch(Exception e)
        {
            Debug.Log("异常信息："+e.Message);
        }
    }

    /// <summary>
    /// 通过Json初始化模型
    /// </summary>
    private void InitJsonFunction() {
        
        //读取Json 文件
        this.Live2dModel3Json = CubismModel3Json.LoadAtPath(Config.ModelPath, BuiltInLoadAtPath);
        if (Live2dModel3Json != null) 
            Live2dCubismModel = Live2dModel3Json.ToModel(true);
        else
            return;
        Debug.Log("model name: " + Live2dCubismModel.name);
        Config.PersonName = Live2dCubismModel.name;
        //Live2d Object
        this.Live2dObject = GameObject.Find(Config.PersonName);
        //添加到已经打开的Live2d列表
        if (Live2dObject == null)
            throw new Exception("Object is null");
        OpeningLive2dModelList.Add(this.Live2dObject);
        //模型物体名字加编号
        this.Live2dObject.name = Config.ModelName + Config.ModelId.ToString();
        //编号++
        Config.ModelId++;
        //模型位置信息
        Live2dObjectTransform = Live2dObject.transform;
        //Parameters Object
        this.Live2dParameters = Live2dObject.transform.Find("Parameters").gameObject;
        if (Live2dParameters == null)
            Debug.Log("Parameters is null");
        else { }
        //Parts Object
        this.Live2dParts = Live2dObject.transform.Find("Parts").gameObject;
        if (Live2dParts == null)
            Debug.Log("Parts is null");
        else { }
        //Drawables Object
        this.Live2dDrawables = Live2dObject.transform.Find("Drawables").gameObject;
        if (Live2dDrawables == null)
            Debug.Log("Drawables is null");
        else { }
    }

    /// <summary>
    /// 生成模型控件列表
    /// </summary>
    private void InitModelHarmonic()
    {
        string directoryPath = Config.DirectoryPath;
        if(directoryPath == null)return;
        if (Live2dObject == null) return;
        CubismModel cubismModel = Live2dObject.FindCubismModel();
        if (cubismModel == null) return;

        Dictionary<string,string> parameterDic=new();
        Dictionary<string,string> partDic = new();
        Dictionary<string,string> drawableDic=new();
        //控件偏移
        for (int i = 0; i < cubismModel.Parameters.Length; ++i)
        {
            CubismParameter cubismParameter = cubismModel.Parameters[i];
            parameterDic[cubismParameter.name] = ("\""+ cubismParameter.DefaultValue.ToString() + "," + cubismParameter.MinimumValue.ToString() + "," + cubismParameter.MaximumValue.ToString()+ "\"");
        }
        //控件透明度
        for(int i=0;i<cubismModel.Parts.Length; ++i)
        {
            CubismPart cubismPart = cubismModel.Parts[i];
            partDic[cubismPart.name] = ("\"" + 1.0f.ToString() + "," + 0.0f.ToString() + "," + 1.0f.ToString() + "\"");
        }
        //控件显示隐藏
        for(int i = 0; i < cubismModel.Drawables.Length; ++i)
        {
            CubismDrawable cubismDrawable = cubismModel.Drawables[i];
            drawableDic[cubismDrawable.name] = "\"1\"";
        }
        Dictionary<string, Dictionary<string, string>> data=new();
        data.Add(Config.BaseParameter, parameterDic);
        data.Add(Config.BasePart, partDic);
        data.Add(Config.BaseDrawable, drawableDic);
        string iniPath = directoryPath + Config.ModelConfigFileName;
        FileOperate.WriteIniFile(data,iniPath);
    }

    private void InitModelExpression()
    {
        string directoryPath = Config.DirectoryPath; 
        string[] files = Directory.GetFiles(directoryPath, "*.exp3.json", SearchOption.AllDirectories);
#if PRIMORDIAL
        CubismExpressionController cubismExpressionController = Live2dObject.GetComponent<CubismExpressionController>();
        if (cubismExpressionController == null) return;
        cubismExpressionController.ExpressionsList =(CubismExpressionList)ScriptableObject.CreateInstance("CubismExpressionList");
        cubismExpressionController.ExpressionsList.CubismExpressionObjects = new CubismExpressionData[0];
        for (int i = 0; i < files.Length; i++)
        {
            string json = File.ReadAllText(files[i]);
            CubismExp3Json cubismExp3Json = CubismExp3Json.LoadFrom(json);
            CubismExpressionData cubismExpressionData = CubismExpressionData.CreateInstance(cubismExp3Json);
            cubismExpressionData.Type = "Live2D Expression";
            int index = cubismExpressionController.ExpressionsList.CubismExpressionObjects.Length;
            Array.Resize(ref cubismExpressionController.ExpressionsList.CubismExpressionObjects, index + 1);
            cubismExpressionController.ExpressionsList.CubismExpressionObjects[index] = cubismExpressionData;
        }
#else
        for (int i = 0; i < files.Length; i++)
        {
            string json = File.ReadAllText(files[i]);
            CubismExp3Json cubismExp3Json = CubismExp3Json.LoadFrom(json);
            CubismExpressionData cubismExpressionData = CubismExpressionData.CreateInstance(cubismExp3Json);
            cubismExpressionData.Type = "Live2D Expression";
            ExpItemsDic[i] = cubismExpressionData;
        }
#endif

        Dictionary<string, string> expressionDic = new();
        for(int i=0;i<files.Length;i++)
            expressionDic[FileOperate.GetFileNameWithoutExtension(files[i])]=i.ToString();
        Dictionary<string, Dictionary<string, string>> data = new();
        data.Add(Config.BaseExpression, expressionDic);
        string iniPath = directoryPath + Config.ModelConfigFileName;
        FileOperate.WriteIniFile(data, iniPath);
    }
    private void InitModelPhysics()
    {
        CubismPhysicsController cubismPhysicsController = Live2dObject.GetComponent<CubismPhysicsController>();

        string directoryPath = Config.DirectoryPath;
        string[] files = Directory.GetFiles(directoryPath, "*.physics3.json", SearchOption.AllDirectories);
        if (files.Length == 0) return;
        Debug.Log("物理json路径: " + files[0]);
        string json = File.ReadAllText(files[0]);
        CubismPhysics3Json cubismPhysics3Json= CubismPhysics3Json.LoadFrom(json);
        cubismPhysicsController.Initialize(cubismPhysics3Json.ToRig());

    }
    private void InitModelFadeAndMotion()
    {
        return;
        string directoryPath = Config.DirectoryPath;
        string[] files = Directory.GetFiles(directoryPath, "*.motion3.json", SearchOption.AllDirectories);

        CubismFadeController cubismFadeController = Live2dObject.GetComponent<CubismFadeController>();
        cubismFadeController.CubismFadeMotionList = (CubismFadeMotionList)ScriptableObject.CreateInstance("CubismFadeMotionList");
        cubismFadeController.CubismFadeMotionList.CubismFadeMotionObjects = new CubismFadeMotionData[0];
        cubismFadeController.CubismFadeMotionList.MotionInstanceIds = new int[0];
        for (int i = 0; i < files.Length; i++)
        {
            string json = File.ReadAllText(files[i]);
            CubismMotion3Json cubismMotion3Json = CubismMotion3Json.LoadFrom(json);
            AnimationClip animationClip = cubismMotion3Json.ToAnimationClip();
            if (animationClip == null)
            {
                Debug.LogError("AnimationClip is null.");
                return;
            }
            //animationClip.legacy = false;
            Debug.Log("AnimationClip Name: " + animationClip.name+
            "Legacy: " + animationClip.legacy+
            "Frame Rate: " + animationClip.frameRate +
            "Length: " + animationClip.length +
            "Is Looping: " + animationClip.isLooping +
            "Is Human Motion: " + animationClip.isHumanMotion +
            "Is Empty: " + animationClip.empty +
            "Wrap Mode: " + animationClip.wrapMode +
            "刚创建时 legacy属性 :" + animationClip.legacy);
            
            //Debug.LogError("刚创建时 legacy属性 :" + animationClip.legacy);

            int index = cubismFadeController.CubismFadeMotionList.CubismFadeMotionObjects.Length;
            {
                var sourceAnimEvents = animationClip.events;
                AnimationEvent instanceIdEvent = null;
                for (int j = 0; j < sourceAnimEvents.Length; ++j)
                {
                    if (sourceAnimEvents[j].functionName != "InstanceId")
                        continue;
                    instanceIdEvent = sourceAnimEvents[j];
                    break;
                }
                if (instanceIdEvent == null){
                    instanceIdEvent = new AnimationEvent();
                    Array.Resize(ref sourceAnimEvents, sourceAnimEvents.Length + 1);
                    sourceAnimEvents[sourceAnimEvents.Length - 1] = instanceIdEvent;
                }
                instanceIdEvent.time = 0;
                instanceIdEvent.functionName = "InstanceId";
                instanceIdEvent.intParameter = index;
                instanceIdEvent.messageOptions = SendMessageOptions.DontRequireReceiver;
                animationClip.events = sourceAnimEvents;
            }
            CubismFadeMotionData cubismFadeMotionData = CubismFadeMotionData.CreateInstance(
                cubismMotion3Json,
                FileOperate.GetFileNameWithoutExtension(files[i]),
                animationClip.length) ;
            AnimItemsDic[index] = animationClip;
            Array.Resize(ref cubismFadeController.CubismFadeMotionList.CubismFadeMotionObjects, index + 1);
            Array.Resize(ref cubismFadeController.CubismFadeMotionList.MotionInstanceIds, index + 1);
            cubismFadeController.CubismFadeMotionList.CubismFadeMotionObjects[index] = cubismFadeMotionData;
            cubismFadeController.CubismFadeMotionList.MotionInstanceIds[index] = index;
        }
        Dictionary<string, string> motionDic = new();
        for (int i = 0; i < files.Length; i++)
            motionDic[FileOperate.GetFileNameWithoutExtension(files[i])] = i.ToString();
        Dictionary<string, Dictionary<string, string>> data = new();
        data.Add(Config.BaseMotion, motionDic);
        string iniPath = directoryPath + Config.ModelConfigFileName;
        FileOperate.WriteIniFile(data, iniPath);
    }

   

    // 设置模型大小
    private void SetModelSize(Vector3 vector3)
    {
        Live2dObject.transform.localScale = vector3;
    }
    // 设置模型位置
    private void SetModelTransformPosition(Vector3 vector3)
    {
        Live2dObject.transform.position = vector3;
    }
    //设置模型旋转
    private void SetModelTransformRotation(Vector3 vector3)
    {
        Live2dObject.transform.rotation = Quaternion.Euler(vector3.x, vector3.y, vector3.z);
    }
    public void AddParameterDic(string itemName, float value)
    {
        ParamItemsDic[itemName] = value;
    }
    public void AddPartList(string itemName, float value)
    {
        PartItemsList.Add(Tuple.Create(itemName, value));
    }
    public void AddDrawableList(string itemName, float value)
    {
        DrawItemsList.Add(Tuple.Create(itemName, value));
    }


    //根据ID播放表情
    public void SendExpression(int index)
    {


#if PRIMORDIAL
        ExpressionIndex=index;
        if (Live2dObject == null) return;
        Debug.Log("设置表情编号: "+ ExpressionIndex.ToString());
        Live2dObject.GetComponent<CubismExpressionPreview>().ChangeExpression(ExpressionIndex);
#else
        if (index < 0 || index >= ExpItemsDic.Count) return;
        CubismExpressionData cubismExpressionData = ExpItemsDic[index];
        Expression expression = GetComponent<Expression>();
        if (expression == null) return;
        List<ExpressionItem> expressionItemList = new List<ExpressionItem>();
        for (int i = 0; i < cubismExpressionData.Parameters.Length; ++i)
        {
            CubismExpressionData.SerializableExpressionParameter sep = cubismExpressionData.Parameters[i];
            ExpressionItem expressionItem = new ExpressionItem();
            expressionItem.name = sep.Id;
            expressionItem.initialValue = Live2dCubismModel.Parameters.FindById(sep.Id).Value;
            expressionItem.value = expressionItem.initialValue;
            expressionItem.targetValue = sep.Value;
            expressionItem.intime = (cubismExpressionData.FadeInTime==0f ? 0.5f : cubismExpressionData.FadeInTime);
            expressionItem.outtime = (cubismExpressionData.FadeOutTime == 0f ? 0.5f : cubismExpressionData.FadeOutTime);
            expressionItemList.Add(expressionItem);
        }
        expression.AddExpressionItemDic(expressionItemList);
#endif
    }
    //根据ID播放动画
    public void SendAnimation(int index)
    {
        return;
        if (AnimItemsDic[index].empty) Debug.LogError("动画资源播放时检查为空!");
        Debug.Log("动画的播放: "+ AnimItemsDic[index].name);
        CubismMotionController cubismMotionController = Live2dObject.GetComponent<CubismMotionController>();
        if(cubismMotionController == null) return;
        if (!AnimItemsDic.ContainsKey(index)) return;
        cubismMotionController.PlayAnimation(AnimItemsDic[index], isLoop: false, priority: CubismMotionPriority.PriorityNormal);
    }

    /// <summary>
    /// 初始模型控件协调
    /// </summary>
    public void InitModelParameters()
    {
        if (Live2dCubismModel == null) return;
        for(int i=0;i< Live2dCubismModel.Parameters.Length; ++i)
        {
            CubismParameter cubismParameter = Live2dCubismModel.Parameters[i];
            if (cubismParameter.Value != cubismParameter.DefaultValue)
                AddParameterDic(cubismParameter.name, cubismParameter.DefaultValue);
        }
    }
    /// <summary>
    /// 初始模型控件透明度初始化
    /// </summary>
    public void InitModelParts()
    {
        if (Live2dCubismModel == null) return;
        for (int i = 0; i < Live2dCubismModel.Parts.Length; ++i)
        {
            CubismPart cubismPart = Live2dCubismModel.Parts[i];
            if(cubismPart.Opacity!=1.0f)
                AddPartList(cubismPart.name,1.0f);
        }
    }

    /// <summary>
    /// 初始模型控件显示
    /// </summary>
    public void InitModelDrawables()
    {
        if (Live2dCubismModel == null) return;
        for (int i = 0; i < Live2dCubismModel.Drawables.Length; ++i)
        {
            CubismDrawable cubismDrawable = Live2dCubismModel.Drawables[i];
            if (cubismDrawable.gameObject.activeSelf == false)
                AddDrawableList(cubismDrawable.name, 1.0f);
        }
    }

    /// <summary>
    /// 自定义控件协调初始化
    /// </summary>
    public void InitSelfModelParameters()
    {
        string filePath = Config.DirectoryPath + Config.ModelConfigFileName;
        Dictionary<string, Dictionary<string, string>> dataDic = FileOperate.ParseIniFile(filePath);
        if(!dataDic.ContainsKey(Config.BaseParameterChange))return;
        Dictionary<string, string> parameterDic = dataDic[Config.BaseParameterChange];
        if (Live2dCubismModel == null) return;
        for (int i = 0; i < Live2dCubismModel.Parameters.Length; ++i)
        {
            CubismParameter cubismParameter = Live2dCubismModel.Parameters[i];
            if (parameterDic.ContainsKey(cubismParameter.name))
            {
                AddParameterDic(cubismParameter.name, float.Parse(parameterDic[cubismParameter.name]) / 100f);
                continue;
            }
            if (cubismParameter.Value != cubismParameter.DefaultValue)
                AddParameterDic(cubismParameter.name, cubismParameter.DefaultValue);
        }
    }
    /// <summary>
    /// 自定义控件透明度初始化
    /// </summary>
    public void InitSelfModelParts()
    {
        string filePath = Config.DirectoryPath + Config.ModelConfigFileName;
        Dictionary<string, Dictionary<string, string>> dataDic = FileOperate.ParseIniFile(filePath);
        if (!dataDic.ContainsKey(Config.BasePartChange)) return;
        Dictionary<string, string> partDic = dataDic[Config.BasePartChange];
        if (Live2dCubismModel == null) return;
        for (int i = 0; i < Live2dCubismModel.Parts.Length; ++i)
        {
            CubismPart cubismPart = Live2dCubismModel.Parts[i];
            if (partDic.ContainsKey(cubismPart.name))
            {
                AddPartList(cubismPart.name, float.Parse(partDic[cubismPart.name]) / 100f);
                continue;
            }
            if (cubismPart.Opacity != 1.0f)
                AddPartList(cubismPart.name, 1.0f);
        }
    }

    /// <summary>
    /// 自定义控件渲染初始化
    /// </summary>
    public void InitSelfModelDrawables()
    {
        string filePath = Config.DirectoryPath + Config.ModelConfigFileName;
        Dictionary<string, Dictionary<string, string>> dataDic = FileOperate.ParseIniFile(filePath);
        if (!dataDic.ContainsKey(Config.BaseDrawableChange)) return;
        Dictionary<string, string> drawableDic = dataDic[Config.BaseDrawableChange];
        foreach(KeyValuePair<string,string> item in drawableDic)
            AddDrawableList(item.Key,float.Parse(item.Value));
    }

    private void LateUpdate()
    {
        if(Live2dCubismModel == null) return;
        //控件偏移 & 谐波 要求实时刷新
        foreach (KeyValuePair<string, float> paramPair in ParamItemsDic)
        {
            CubismParameter parameterItem = Live2dCubismModel.Parameters.FindById(paramPair.Key);
            if (parameterItem != null)
                parameterItem.Value= paramPair.Value;
                parameterItem.BlendToValue(CubismParameterBlendMode.Override, paramPair.Value);
        }
        //控件透明度设置
        if (PartItemsList.Count>0)
        {
            Tuple<string, float> partTuple = PartItemsList[0];
            if(partTuple != null)
            {
                CubismPart cubismPart = Live2dCubismModel.Parts.FindById(partTuple.Item1);
                if(cubismPart != null)
                {
                    Debug.Log("控件透明调度: "+ partTuple.Item1+" "+ partTuple.Item2);
                    cubismPart.Opacity = partTuple.Item2;
                }
            }
            PartItemsList.RemoveAt(0);
        }

            //控件显示隐藏执行一次就可以
        if (DrawItemsList.Count > 0)
        {
            Tuple<string, float> drawTuple = DrawItemsList[0];
            if (drawTuple != null) {
                CubismDrawable drawableItem= Live2dCubismModel.Drawables.FindById(drawTuple.Item1);
                if (drawableItem != null)
                {
                    Debug.Log("隐藏或显示变化的控件: " + drawTuple.Item1 + "  " + drawTuple.Item2);
                    drawableItem.gameObject.SetActive(drawTuple.Item2 > 0.5f ? true : false);
                }
            }
            DrawItemsList.RemoveAt(0);
        }
    }


    /// <summary>
    /// 初始化模型脚本以及参数信息
    /// </summary>
    private void InitModelFunction()
    {
        //设置默认挂载脚本
        AddComponentFunction();
        //设置模型大小
        SetModelSize(new Vector3(Config.ScaleProportionItem.Param, Config.ScaleProportionItem.Param, 1));
        //设置图层正确显示
        Live2dObject.GetComponent<CubismRenderController>().SortingMode = CubismSortingMode.BackToFrontOrder;
        //Live2dObject.GetComponent<CubismRenderController>().SortingOrder = (Config.ModelId - 1) * 500;//多个Model设置渲染优先级

        //控件视线跟踪固定参数
        Live2dObject.GetComponent<CubismLookController>().BlendMode = CubismParameterBlendMode.Override;
        Live2dObject.GetComponent<CubismEyeBlinkController>().BlendMode = CubismParameterBlendMode.Override;
        Live2dObject.GetComponent<CubismMouthController>().BlendMode = CubismParameterBlendMode.Override;
        Live2dObject.GetComponent<CubismAudioMouthInput>().SamplingQuality = CubismAudioSamplingQuality.Maximum;
        Live2dObject.GetComponent<CubismPoseController>().defaultPoseIndex = 0;
        FindTransform("PAX", out TransformParamAngleX);
        FindTransform("PAY", out TransformParamAngleY);
        FindTransform("PAZ", out TransformParamAngleZ);
        FindTransform("PBAX", out TransformParamBodyAngleX);
        FindTransform("PBAY", out TransformParamBodyAngleY);
        FindTransform("PBAZ", out TransformParamBodyAngleZ);
        FindTransform("PEBX", out TransformParamEyeBallX);
        FindTransform("PEBY", out TransformParamEyeBallY);

        if (TransformParamAngleX != null) TransformParamAngleX.gameObject.GetComponent<CubismLookParameter>().Axis = CubismLookAxis.X;
        if (TransformParamAngleY != null) TransformParamAngleY.gameObject.GetComponent<CubismLookParameter>().Axis = CubismLookAxis.Y;
        if (TransformParamAngleZ != null) TransformParamAngleZ.gameObject.GetComponent<CubismLookParameter>().Axis = CubismLookAxis.Z;

        if (TransformParamBodyAngleX != null) TransformParamBodyAngleX.gameObject.GetComponent<CubismLookParameter>().Axis = CubismLookAxis.X;
        if (TransformParamBodyAngleY != null) TransformParamBodyAngleY.gameObject.GetComponent<CubismLookParameter>().Axis = CubismLookAxis.Y;
        if (TransformParamBodyAngleZ != null) TransformParamBodyAngleZ.gameObject.GetComponent<CubismLookParameter>().Axis = CubismLookAxis.Z;

        if (TransformParamEyeBallX != null) TransformParamEyeBallX.gameObject.GetComponent<CubismLookParameter>().Axis = CubismLookAxis.X;
        if (TransformParamEyeBallY != null) TransformParamEyeBallY.gameObject.GetComponent<CubismLookParameter>().Axis = CubismLookAxis.Y;

        //根据Config刷新脚本参数
        UpdateModelCondition();
    }
    /// <summary>
    /// 更新模型参数
    /// </summary>
    public void UpdateModelCondition()
    {
        Debug.Log("根据Config刷新模型列表的通用参数");
        //更新模型大小
        SetModelSize(new Vector3(Config.ScaleProportionItem.Param, Config.ScaleProportionItem.Param, 1));
        //更新模型位置
        SetModelTransformPosition(new Vector3(Config.PositionXItem.Param, Config.PositionYItem.Param, 0));
        //更新模型旋转
        SetModelTransformRotation(new Vector3(Config.RotationRXItem.Param, Config.RotationRYItem.Param, Config.RotationRZItem.Param));

        gameObject.transform.Find("Target").GetComponent<LookMouse>().SetLookMouse(Config.IsLookMouse);

        //更新视线追踪参数
        if (TransformParamAngleX != null) TransformParamAngleX.gameObject.GetComponent<CubismLookParameter>().Factor = Config.ParamAngleItem.Param;
        if (TransformParamAngleY != null) TransformParamAngleY.gameObject.GetComponent<CubismLookParameter>().Factor = Config.ParamAngleItem.Param;
        if (TransformParamAngleZ != null) TransformParamAngleZ.gameObject.GetComponent<CubismLookParameter>().Factor = Config.ParamAngleItem.Param;

        if (TransformParamBodyAngleX != null) TransformParamBodyAngleX.gameObject.GetComponent<CubismLookParameter>().Factor = Config.ParamBodyAngleItem.Param;
        if (TransformParamBodyAngleY != null) TransformParamBodyAngleY.gameObject.GetComponent<CubismLookParameter>().Factor = Config.ParamBodyAngleItem.Param;
        if (TransformParamBodyAngleZ != null) TransformParamBodyAngleZ.gameObject.GetComponent<CubismLookParameter>().Factor = Config.ParamBodyAngleItem.Param;

        if (TransformParamEyeBallX != null) TransformParamEyeBallX.gameObject.GetComponent<CubismLookParameter>().Factor = Config.ParamEyeBallItem.Param;
        if (TransformParamEyeBallY != null) TransformParamEyeBallY.gameObject.GetComponent<CubismLookParameter>().Factor = Config.ParamEyeBallItem.Param;

        //设置人物看向鼠标速度
        if (Live2dObject.GetComponent<CubismLookController>() != null) Live2dObject.GetComponent<CubismLookController>().Damping = Config.DampingItem.Param;
        if (Live2dObject.GetComponent<CubismLookController>() != null) Live2dObject.GetComponent<CubismLookController>().Center = gameObject.transform;

        //设置眨眼参数
        if (Live2dObject.GetComponent<CubismAutoEyeBlinkInput>() != null) Live2dObject.GetComponent<CubismAutoEyeBlinkInput>().Mean = Config.MeanItem.Param;
        if (Live2dObject.GetComponent<CubismAutoEyeBlinkInput>() != null) Live2dObject.GetComponent<CubismAutoEyeBlinkInput>().MaximumDeviation = Config.MaximumDeviationItem.Param;
        if (Live2dObject.GetComponent<CubismAutoEyeBlinkInput>() != null) Live2dObject.GetComponent<CubismAutoEyeBlinkInput>().Timescale = Config.TimescaleItem.Param;

        //设置张嘴参数
        if (Live2dObject.GetComponent<CubismMouthController>() != null) Live2dObject.GetComponent<CubismMouthController>().MouthOpening = Config.MouthOpeningItem.Param;
        //设置音频参数
        if (Live2dObject.GetComponent<CubismAudioMouthInput>() != null) Live2dObject.GetComponent<CubismAudioMouthInput>().Gain = Config.GainItem.Param;
        if (Live2dObject.GetComponent<CubismAudioMouthInput>() != null) Live2dObject.GetComponent<CubismAudioMouthInput>().Smoothing = Config.SmoothingItem.Param;
    }
    /// <summary>
    /// 默认挂载脚本或物体
    /// </summary>
    private void AddComponentFunction()
    {
        //挂载默认的脚本
        if (Live2dObject.GetComponent<CubismUpdateController>() == null)Live2dObject.AddComponent<CubismUpdateController>();
        if (Live2dObject.GetComponent<CubismParameterStore>() == null) Live2dObject.AddComponent<CubismParameterStore>();
        if (Live2dObject.GetComponent<CubismPoseController>() == null) Live2dObject.AddComponent<CubismPoseController>();
        if (Live2dObject.GetComponent<CubismExpressionController>() == null) Live2dObject.AddComponent<CubismExpressionController>();
        if (Live2dObject.GetComponent<CubismAutoEyeBlinkInput>() == null) Live2dObject.AddComponent<CubismAutoEyeBlinkInput>();
        if (Live2dObject.GetComponent<CubismMouthController>() == null) Live2dObject.AddComponent<CubismMouthController>();
        if (Live2dObject.GetComponent<CubismAudioMouthInput>() == null) Live2dObject.AddComponent<CubismAudioMouthInput>();
        if (Live2dObject.GetComponent<CubismLookController>() == null) Live2dObject.AddComponent<CubismLookController>();
        if (Live2dObject.GetComponent<CubismEyeBlinkController>() == null) Live2dObject.AddComponent<CubismEyeBlinkController>();
        if (Live2dObject.GetComponent<CubismExpressionPreview>() == null) Live2dObject.AddComponent <CubismExpressionPreview>();
        if (Live2dObject.GetComponent<ModelUserEvent>() == null) Live2dObject.AddComponent<ModelUserEvent>();
        if (Live2dObject.GetComponent<CubismPhysicsController>() == null) Live2dObject.AddComponent<CubismPhysicsController>();


        InitModelPhysics();

        //Live2dObject.GetComponent<CubismPhysicsController>().Initialize();


        ////注意这里务必要在挂载CubismMotionController前设置好CubismFadeController
        //if (Live2dObject.GetComponent<CubismFadeController>() == null)
        //{
        //    Live2dObject.AddComponent<CubismFadeController>();

        //    InitModelFadeAndMotion();
        //}
        //if (Live2dObject.GetComponent<CubismMotionController>() == null) Live2dObject.AddComponent<CubismMotionController>();

        foreach (string itemname in Config.NeedAddCubismLookParameterObjectStrings)
        {
            Transform transformItem = Live2dParameters.transform.Find(itemname);
            if(transformItem != null)
            {
                if(transformItem.gameObject.GetComponent<CubismLookParameter>()==null)
                    transformItem.gameObject.AddComponent<CubismLookParameter>();
            }
        }
        //张嘴
        foreach(string item  in Config.NeedAddCubismMouthParameterObjectStrings)
        {
            Transform transformParamMouthOpenY = Live2dParameters.transform.Find(item);
            if (transformParamMouthOpenY != null)
            {
                if (transformParamMouthOpenY.gameObject.GetComponent<CubismMouthParameter>() == null)
                    transformParamMouthOpenY.gameObject.AddComponent<CubismMouthParameter>();
            }
        }
        //眨眼
        foreach (string item in Config.NeedAddCubismEyeBlinkParameterObjectStrings)
        {
            Transform TransformParamEyeOpen = Live2dParameters.transform.Find(item);
            if (TransformParamEyeOpen != null)
            {
                if (TransformParamEyeOpen.gameObject.GetComponent<CubismEyeBlinkParameter>() == null)
                    TransformParamEyeOpen.gameObject.AddComponent<CubismEyeBlinkParameter>();
            }
        }

        //默认挂载的控件
        Transform transformTarget = gameObject.transform.Find("Target");
        if(transformTarget != null)
        {
            Live2dObject.GetComponent<CubismLookController>().Target = transformTarget.gameObject;
        }
        Transform transformAudioInput = gameObject.transform.Find("Audio");
        if(transformAudioInput != null)
        {
            Live2dObject.GetComponent<CubismAudioMouthInput>().AudioInput = transformAudioInput.gameObject.GetComponent<AudioSource>();
        }
    }
    /// <summary>
    /// 工具函数获取需要鼠标跟踪的控件
    /// </summary>
    void FindTransform(string key, out Transform result)
    {
        result = null;
        if (Config.keyValueParameters.TryGetValue(key, out var parameters))
        {
            foreach (var item in parameters)
            {
                result = Live2dParameters.transform.Find(item);
                if (result != null)
                    break;
            }
        }
    }

    /// <summary>
    /// 删除模型
    /// </summary>
    public void DestroyModel()
    {
        Destroy(Live2dParameters);
        Destroy(Live2dParts);
        Destroy(Live2dDrawables);
        Destroy(Live2dCubismModel);

        Destroy(TransformParamAngleX);
        Destroy(TransformParamAngleY);
        Destroy(TransformParamAngleZ);
        Destroy(TransformParamBodyAngleX);
        Destroy(TransformParamBodyAngleY);
        Destroy(TransformParamBodyAngleZ);
        Destroy(TransformParamEyeBallX);
        Destroy(TransformParamEyeBallY);


        while (OpeningLive2dModelList.Count > 0)
        {
            GameObject obj = OpeningLive2dModelList[0];
            Destroy(obj);
            OpeningLive2dModelList.RemoveAt(0);
        }
        OpeningLive2dModelList.Clear();
        ParamItemsDic.Clear();
        PartItemsList.Clear();
        DrawItemsList.Clear();
        AnimItemsDic.Clear();

#if PRIMORDIAL
#else
        ExpItemsDic.Clear();
#endif
        Resources.UnloadUnusedAssets();

    }

    /// <summary>
    /// 通过json加载模型信息
    /// </summary>
    private static object BuiltInLoadAtPath(Type assetType,string absolutePath)
    {
        if (assetType == typeof(byte[]))
        {
            return File.ReadAllBytes(absolutePath);
        }
        else  if(assetType == typeof(string))
        {
            return File.ReadAllText(absolutePath);
        }
        else if( assetType == typeof(Texture2D)) 
        {
            Texture2D texture2D = new Texture2D(1, 1);
            texture2D.LoadImage(File.ReadAllBytes(absolutePath));
            return texture2D;
        }
        throw new NotSupportedException();
    }


}
