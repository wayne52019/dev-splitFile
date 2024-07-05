using UnityEngine;
using System.IO;

public class Example : MonoBehaviour
{
    System.Threading.CancellationTokenSource source;

    [Header("分割大小(單位為byte)")]
    [SerializeField]
    int chunkSizeInBytes;

    /// <summary>
    /// 「分割檔案(存記憶體)」範例
    /// </summary>
    public async void testSplitFileToChunks()
    {
        void loading(float f)
        {
            Debug.Log(f);
        }

        //取消令牌要實例化新的，否則會發生持續使用被取消過的令牌，會造成每次執行都被直接取消
        source = new System.Threading.CancellationTokenSource();

        var result = await AR2VR.SplitFileManager.SplitFileToChunks(Application.streamingAssetsPath + "/SplitFileTest/test.mp4", chunkSizeInBytes, source, loading);

        Debug.Log(result.state);


        switch (result.state)
        {
            case AR2VR.SplitFileManager.State.success:
                byte[][] b = new byte[result.result.Length][];

                int index = 0;

                foreach(var item in result.result)
                {
                    b[index] = item.bts;

                    index++;
                }

                File.WriteAllBytes(Application.streamingAssetsPath + "/SplitFileTest/test2.mp4", AR2VR.SplitFileManager.Reorganization(b));
                break;
            case AR2VR.SplitFileManager.State.fail:
                break;
            case AR2VR.SplitFileManager.State.cancel:
                break;
        }
    }

    /// <summary>
    /// 「分割檔案並另存新檔」範例
    /// </summary>
    public async void testSplitFile()
    {
        void loading(float f)
        {
            Debug.Log(f);
        }

        //取消令牌要實例化新的，否則會發生持續使用被取消過的令牌，會造成每次執行都被直接取消
        source = new System.Threading.CancellationTokenSource();

        var result = await AR2VR.SplitFileManager.SplitFile(Application.streamingAssetsPath + "/SplitFileTest/test.mp4", chunkSizeInBytes, source, loading);

        Debug.Log(result.state);

        switch (result.state)
        {
            case AR2VR.SplitFileManager.State.success:
                byte[][] b =new byte[result.result.Length][];

                int index = 0;

                foreach(var item in result.result)
                {
                    b[index] = File.ReadAllBytes(item.path);
                    index++;
                }

                File.WriteAllBytes(Application.streamingAssetsPath + "/SplitFileTest/test2.mp4", AR2VR.SplitFileManager.Reorganization(b));

                break;
            case AR2VR.SplitFileManager.State.fail:
                break;
            case AR2VR.SplitFileManager.State.cancel:
                break;
        }
    }

    private void OnApplicationQuit()
    {
        if (File.Exists(Application.streamingAssetsPath + "/SplitFileTest/test2.mp4"))
            File.Delete(Application.streamingAssetsPath + "/SplitFileTest/test2.mp4");
    }

    /// <summary>
    /// 取消範例
    /// </summary>
    public void cancel()
    {
        source.Cancel();
    }
}
