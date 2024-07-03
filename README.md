# 檔案分割模組

## 函式說明

### 分割檔案(存記憶體) 不推薦使用：

```C#
/// <summary>
/// 分割檔案(存記憶體)
/// </summary>
/// <param name="filePath">要分割檔案路徑</param>
/// <param name="chunkSizeInBytes">切割大小</param>
/// <param name="source">取消令牌</param>
/// <param name="loading">進度事件</param>
/// <returns></returns>
public static async Task<SplitFileToChunksResult> SplitFileToChunks(string filePath, int chunkSizeInBytes, CancellationTokenSource source = null, Action<float> loading = null)
```

#### 回傳結果類(SplitFileToChunksResult) 變數說明：

##### 分割處理狀態

```C#
public enum State
{
  //成功
  success,
  //失敗
  fail,
  //取消
  cancel
}

public State state
{
  get
  {
    return _state;
  }
}

```

##### 分割位元組

```C#
public byte[][] result
{
  get
  {
    return _result;
  }
}
```

### 分割檔案並另存新檔：

```C#
/// <summary>
/// 分割檔案並另存新檔
/// </summary>
/// <param name="filePath">要分割檔案路徑</param>
/// <param name="chunkSizeInBytes">切割大小</param>
/// <param name="source">取消令牌</param>
/// <param name="loading">進度事件</param>
public static async Task<SplitFileResult> SplitFile(string filePath, int chunkSizeInBytes,CancellationTokenSource source = null, Action<float> loading = null)
```

#### 回傳結果類(SplitFileResult) 變數說明：

##### 分割處理狀態

```C#
public enum State
{
  //成功
  success,
  //失敗
  fail,
  //取消
  cancel
}

public State state
{
  get
  {
    return _state;
  }
}

```

##### 分割檔路徑

```C#
public string[] result
{
  get
  {
    return _result;
  }
}
```
