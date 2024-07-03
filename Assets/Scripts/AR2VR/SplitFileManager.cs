using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace AR2VR
{
    public class SplitFileManager
    {
        public static string version
        {
            get
            {
                return "1";
            }
        }

        public enum State
        {
            //成功
            success,
            //失敗
            fail,
            //取消
            cancel
        }

        /// <summary>
        /// 「分割檔案(存記憶體)」回傳結果類
        /// </summary>
        public class SplitFileToChunksResult
        {
            public SplitFileToChunksResult(State s,byte[][] r)
            {
                _state = s;

                _result = r;
            }

            public State state
            {
                get
                {
                    return _state;
                }
            }
            State _state;

            public byte[][] result
            {
                get
                {
                    return _result;
                }
            }
            byte[][] _result;
        }
        /// <summary>
        /// 分割檔案(存記憶體)
        /// </summary>
        /// <param name="filePath">要分割檔案路徑</param>
        /// <param name="chunkSizeInBytes">切割大小</param>
        /// <param name="source">取消令牌</param>
        /// <param name="loading">進度事件</param>
        /// <returns></returns>
        public static async Task<SplitFileToChunksResult> SplitFileToChunks(string filePath, int chunkSizeInBytes, CancellationTokenSource source = null, Action<float> loading = null)
        {
            byte[][] chunks = null;

            State state = State.success;

            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var amount = GetChunkAmout(fileStream, chunkSizeInBytes);

                    chunks = new byte[amount][];

                    byte[] buffer = new byte[chunkSizeInBytes];

                    int index = 0;

                    int bytesRead;

                    loading?.Invoke((float)index / (float)amount);

                    while ((bytesRead = await (source != null ? fileStream.ReadAsync(buffer, 0, chunkSizeInBytes, source.Token) : fileStream.ReadAsync(buffer, 0, chunkSizeInBytes))) > 0)
                    {
                        byte[] chunk;

                        // If the last chunk is smaller than the buffer, we need to trim the buffer to the correct size
                        if (bytesRead < chunkSizeInBytes)
                        {
                            chunk = new byte[bytesRead];
                            Array.Copy(buffer, chunk, bytesRead);
                        }
                        else
                        {
                            chunk = (byte[])buffer.Clone();
                        }

                        chunks[index] = chunk;

                        index++;

                        loading?.Invoke((float)index / (float)amount);
                    }
                }
            }
            catch(Exception e)
            {
                chunks = null;

                //取消
                if (e.Message == "A task was canceled.")
                    state = State.cancel;
                //發生錯誤失敗
                else
                    state = State.fail;
            }

            return new SplitFileToChunksResult(state, chunks);
        }

        /// <summary>
        /// 「分割檔案並另存新檔」回傳結果類
        /// </summary>
        public class SplitFileResult
        {
            public SplitFileResult(State s, string[] r)
            {
                _state = s;

                _result = r;
            }

            public State state
            {
                get
                {
                    return _state;
                }
            }
            State _state;

            public string[] result
            {
                get
                {
                    return _result;
                }
            }
            string[] _result;
        }
        /// <summary>
        /// 分割檔案並另存新檔
        /// </summary>
        /// <param name="filePath">要分割檔案路徑</param>
        /// <param name="chunkSizeInBytes">切割大小</param>
        /// <param name="source">取消令牌</param>
        /// <param name="loading">進度事件</param>
        /// <returns></returns>
        public static async Task<SplitFileResult> SplitFile(string filePath, int chunkSizeInBytes,CancellationTokenSource source = null, Action<float> loading = null)
        {
            string[] chunkPaths = null;

            State state = State.success;

            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var amount = GetChunkAmout(fileStream, chunkSizeInBytes);

                    byte[] buffer = new byte[chunkSizeInBytes];

                    chunkPaths = new string[amount];

                    int index = 0;

                    int bytesRead;

                    loading?.Invoke((float)index / (float)amount);

                    while ((bytesRead = await (source != null ? fileStream.ReadAsync(buffer, 0, chunkSizeInBytes, source.Token) : fileStream.ReadAsync(buffer, 0, chunkSizeInBytes))) > 0)
                    {
                        string chunkFileName = $"{filePath}.part{index}";

                        using (FileStream chunkFile = new FileStream(chunkFileName, FileMode.Create))
                        {
                            await chunkFile.WriteAsync(buffer, 0, bytesRead);
                        }
                        chunkPaths[index] = chunkFileName;

                        index++;

                        loading?.Invoke((float)index / (float)amount);
                    }
                }
            }
            catch (Exception e)
            {
                chunkPaths = null;

                //取消
                if (e.Message == "A task was canceled.")
                    state = State.cancel;
                //發生錯誤失敗
                else
                    state = State.fail;
            }

            return new SplitFileResult(state, chunkPaths);
        }

        /// <summary>
        /// 檔案重組(測試用)
        /// </summary>
        /// <param name="split"></param>
        /// <returns></returns>
        public static byte[] Reorganization(byte[][] split)
        {

            int length = 0;

            //取得長度
            foreach (var item in split)
            {
                length += item.Length;
            }

            byte[] b = new byte[length];

            int index = 0;

            //重組檔案
            foreach (var item in split)
            {
                foreach (var item2 in item)
                {
                    b[index] = item2;
                    index++;
                }
            }

            return b;
        }

        /// <summary>
        /// 取得檔案分割長度
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="chunkSizeInBytes"></param>
        /// <returns></returns>
        static long GetChunkAmout(FileStream fileStream,int chunkSizeInBytes)
        {
            return (fileStream.Length / chunkSizeInBytes) + (fileStream.Length % chunkSizeInBytes != 0 ? 1 : 0);
        }
    }
}

