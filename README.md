# Environment Query for Unity

基于 Unreal Engine 5.7 EQS 源码实现的 Unity 版本环境查询系统。

## 简介

Environment Query System (EQS) 是 Unity 的一种AI决策系统，用于帮助 AI 角色找到最佳位置或目标。它通过组合多个生成器（Generator）和测试（Test）来评估场景中的位置，并返回最符合条件的点。

## 功能特性

### 生成器 (Generators)

| 生成器 | 描述 |
|--------|------|
| **OnCircle** | 在搜索中心周围圆形分布生成测试点 |
| **Donut** | 圆环形分布生成测试点 |
| **SimpleGrid** | 网格形式生成测试点 |
| **ActorsOfClass** | 获取特定类别的所有 Actor |
| **ProjectedPoints** | 基于投射点生成测试位置 |

### 测试 (Tests)

| 测试 | 描述 |
|------|------|
| **Distance** | 计算到上下文的距离 |
| **Dot** | 计算点积（角度相关） |
| **Trace** | 射线检测（障碍物检测） |
| **PathFinding** | 寻路测试（NavMesh） |

### 上下文 (Contexts)

- **Querier**: 执行查询的 AI 角色
- **Item**: 正在评估的测试点

## 项目结构

```
Assets/EnvironmentQuery/
├── Scripts/
│   ├── Editor/                    # 编辑器相关
│   │   ├── EnvQueryInspector.cs
│   │   └── EnvQueryContextDrawer.cs
│   ├── EnvQueryContexts/          # 上下文实现
│   │   ├── EnvQueryContext_Querier.cs
│   │   ├── EnvQueryContext_Item.cs
│   │   └── EnvQueryContext_FindByName.cs
│   ├── EnvQueryGenerator/         # 生成器实现
│   │   ├── EnvQueryGenerator.cs
│   │   ├── EnvQueryGeneratorOnCircle.cs
│   │   ├── EnvQueryGeneratorDonut.cs
│   │   ├── EnvQueryGeneratorSimpleGrid.cs
│   │   ├── EnvQueryGeneratorActorsOfClass.cs
│   │   └── EnvQueryGenerator_ProjectedPoints.cs
│   ├── EnvQueryTests/             # 测试实现
│   │   ├── EnvQueryTest.cs
│   │   ├── EnvQueryTestDistance.cs
│   │   ├── EnvQueryTestDot.cs
│   │   ├── EnvQueryTestTrace.cs
│   │   └── EnvQueryTestPathFinding.cs
│   ├── EnvQueryManager.cs         # 查询管理器
│   ├── EnvQueryInstance.cs        # 查询实例
│   ├── EnvQueryRequest.cs         # 查询请求
│   ├── EnvQueryItem.cs            # 查询项
│   └── EnvQueryTypes.cs           # 类型定义
├── Example/                       # 示例场景
│   ├── Sample.unity
│   ├── Scripts/
│   │   ├── PlayerController.cs
│   │   └── NPCController.cs
│   └── Prefabs/
└── ThirdParty/
    ├── NoAlloq/                   # 高性能 Span 操作库
    └── DotNet/
        └── System.Runtime.CompilerServices.Unsafe.dll
```

## 使用方法

### 创建查询

1. 在 Unity 编辑器中右键点击 `Create > Environment Query > EnvQuery`
2. 添加生成器：点击 "+" > 选择生成器类型
3. 添加测试：选中生成器 > 点击 "+" > 选择测试类型
4. 配置测试参数和权重

### 运行查询

```csharp
// 通过管理器运行查询
var queryInstance = EnvQueryManager.Instance.CreateQueryInstance(
    envQueryTemplate, 
    EnvQueryRunMode.AllMatching, 
    ownerGameObject
);

EnvQueryManager.Instance.RunQuery(queryInstance, OnQueryFinished);

void OnQueryFound(List<EnvQueryItem> items)
{
    // 处理查询结果
}
```

## 示例场景

项目包含一个示例场景，展示如何使用 EQS：

- `Assets/EnvironmentQuery/Example/Sample.unity`
- NPC 使用 EQS 寻找最佳位置跟随玩家

## 参考资料

- [Unreal Engine EQS 源码](https://github.com/EpicGames/UnrealEngine/tree/release/Engine/Source/Runtime/AIModule/Private/EnvironmentQuery)
- [Unreal Engine EQS 类参考](https://github.com/EpicGames/UnrealEngine/tree/release/Engine/Source/Runtime/AIModule/Classes/EnvironmentQuery)

## 第三方库

- **NoAlloq**: 高性能 Span 枚举操作库
- **Unity.Collections**: Unity 原生集合库

## 许可证

MIT License - 详见 [LICENSE.txt](LICENSE.txt)

## 截图

<img src="External/ReadMeImages/EnvironmentQueryForUnity01.png" width="50%">
<img src="External/ReadMeImages/EnvironmentQueryForUnity02.png" width="50%">
<img src="External/ReadMeImages/EnvironmentQueryForUnity03.png" width="50%">
