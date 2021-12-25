using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Slover
{
    public static List<int> Solve(int width,int height,int start,int goal)
    {
        Func<int, int, bool> isIn = (x, y) => x >= 0 && x < width && y >= 0 && y < height;
        Func<int, int, int> distance = (x, y) => Mathf.RoundToInt(Mathf.Pow(x - GameManager.Instance._stagemanager.i2x(goal), 2) + Mathf.Pow(y - GameManager.Instance._stagemanager.i2z(goal), 2));
        int maxCost = Mathf.RoundToInt(Mathf.Pow(width, 2) + Mathf.Pow(height, 2));
        Func<int, int> floorCost = c => c / maxCost * maxCost;

        List<int[]> open = new List<int[]>();
        List<int> closed = new List<int>();
        List<int[]> ptr = new List<int[]>();
        List<int> route = new List<int>();

        //オープンにスタートを入れる
        open.Add(new int[] { start, 0 });

        while (open.Count !=0)
        {

            //オープンの一番最初を取り出す
            int[] n_open = open[0];
            open.RemoveAt(0);

            //オープンがゴールなら探索終了
            if(n_open[0] == goal)
            {
                //ゴールを使うから一端保存
                int tofind = goal;
                //実際の経路になる
                route.Add(goal);
                //tofindをstart地点までさかのぼる
                while(tofind != start)
                {
                    //ptrをすべて探す
                    foreach(var p in ptr)
                    {
                        //子供になってるptrを探す
                        if(p[1] == tofind)
                        {
                            //routeの変数の前に入れるため ptrの親の名前を入れる
                            route.Insert(0, p[0]);
                            //親をtofindに入れる
                            tofind = p[0];
                            break;
                        }
                    }
                }
            }else
            {
                //オープンで取り出したものを入れる
                closed.Add(n_open[0]);
                int x = GameManager.Instance._stagemanager.i2x(n_open[0]);
                int z = GameManager.Instance._stagemanager.i2z(n_open[0]);
                //移動できる可能性のあるところ
                int[][] direction = new int[][]
                {
                    new int[]{x-1,z },
                    new int[]{x+1,z },

                    new int[]{x+1,z+1 },
                    new int[]{x+1,z-1 },


                    new int[]{x,z-1 },
                    new int[]{x,z+1 },

                    new int[]{x-1,z-1 },
                    new int[]{x-1,z+1 },
                };

                for(int i=0; i< direction.Length;i++)
                {
                    int idx = GameManager.Instance._stagemanager.xz2i(direction[i][0], direction[i][1]);
                    //壁の内側でかつ壁でないかつ障害物がないか
                    if(isIn(direction[i][0], direction[i][1]) &&
                        CheckBlock(idx))
                    {
                        //openに変える可能性のある変数
                        //インデックス,コストの計算 親のノードまでたどり着いた時の第一のコスト+１ぽ歩いたコスト+第二のコスト
                        int[] m_cost = new int[] { idx, floorCost(n_open[1]) + maxCost + distance(direction[i][0], direction[i][1]) };
                        //オープンに既にｍと同じインデックスを持っているものが入っているかを確認
                        int[] open_node = open.Find(o => o[0] == m_cost[0]);

                        //既にmに相当するものが入っている場合 新しいmのほうがコストが小さい場合
                        if (open_node != null && m_cost[1] < open_node[1])
                        {
                            //入れ替えるために古いノードを削除をする
                            open.Remove(open_node);
                            //mを子供としているptrも削除
                            ptr.RemoveAt(ptr.FindIndex(o => o[1] == m_cost[0]));
                        }
                        //mと同じノードが無かった場合 オープンのほうからすでに削除していた場合 クローズドに入っていない場合
                        if((open_node == null || m_cost[1] < open_node[1]) && closed.Contains(m_cost[0]) == false)
                        {
                            open.Add(m_cost);
                            //親がn　子がm　の親子関係を保存
                            ptr.Add(new int[] { n_open[0], m_cost[0] });
                        }
                    }

                }
            }
            //0がセルのインデックス 1がコスト
            open.Sort((a, b) => a[1] - b[1] == 0 ? a[0] - b[0] : a[1] - b[1]);
        }

        //最終的にルートを返す
        return route;
    }
    /// <summary>
    /// 移動先に障害物がないか
    /// </summary>
    static bool CheckBlock(int index)
    {
        var blocks = GameManager.Instance._stagemanager._objectsData._objects;
        if (blocks[index] == null || !blocks[index].Any(b => b._type != objectType.Item))
        {
            return true;
        }else
        {
            return false;
        }
    }
}
