// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
using AiCodo.Flow.Configs; 

namespace AiCodo.Tests
{
    public class FlowTest
    {
        public FlowTest()
        {
            ApplicationConfig.LocalConfigFolder = "";
            var type = typeof(FlowTestMethods);
            FuncService.Current.RegisterMethod("Plus",type.GetMethod("Plus"));
            FuncService.Current.RegisterMethod("Plus1", type.GetMethod("Plus1"));
        }

        #region 测试基本顺序逻辑
        [Test]
        public void BasicLogic()
        {
            FunctionFlowConfig flow = CreatePlusFlow();

            var context = new FlowContext(new Dictionary<string, object>
            {
                { "x",12},
                { "y",13},
            });

            var result = context.ExecuteFlow(flow).Result;
            Assert.IsNotNull(result);
            var v2 = result.GetInt32("Result");
            Assert.That(v2, Is.EqualTo(26));
        }

        private static FunctionFlowConfig CreatePlusFlow()
        {
            var flow = new FunctionFlowConfig();
            var action1 = new FunctionFlowAction
            {
                ID = "plus",
                FunctionName = "Plus",
                Parameters = new CollectionBase<ActionInputParameter>
                {
                    new ActionInputParameter
                    {
                        Name = "x",
                        IsInherit=false,
                        Expression = "x"
                    },
                    new ActionInputParameter
                    {
                        Name = "y",
                        IsInherit=false,
                        Expression = "y"
                    },
                },
            };
            action1.ResultParameters.Add(new ActionOutputParameter
            {
                Name = "Value",
                ResetInputName = "v"
            });
            flow.Actions.Add(action1);

            var action2 = new FunctionFlowAction
            {
                ID = "plus1",
                FunctionName = "Plus1",
                Parameters = new CollectionBase<ActionInputParameter>
                {
                    new ActionInputParameter
                    {
                        Name = "x",
                        IsInherit=false,
                        Expression = "v"
                    }
                }
            };
            action2.ResultParameters.Add(new ActionOutputParameter
            {
                Name = "Value",
                ResultName = "v2",
                ResetInputName = "v2"
            });
            flow.Actions.Add(action2);

            flow.Parameters.Add(new FlowInputParameter
            {
                Name = "x",
                DefaultValue = "=x"
            });
            flow.Parameters.Add(new FlowInputParameter
            {
                Name = "y",
                DefaultValue = "=y"
            });

            flow.Results.Add(new FlowResultParameter
            {
                Name = "Result",
                Expression = "v2"
            });
            return flow;
        }
        #endregion

        #region 测试分支逻辑
        [Test]
        public void SwitchLogic()
        {
            FunctionFlowConfig flow = CreateSwitchFlow();
            var context = new FlowContext(new Dictionary<string, object>
            {
                { "x",12},
                { "y",13},
            });

            var result = context.ExecuteFlow(flow).Result;
            var v2 = result.GetInt32("Result");
            Assert.That(v2, Is.EqualTo(24));

            context.SetArgs("x", 3);
            context.SetArgs("y", 5);
            result = context.ExecuteFlow(flow).Result;
            v2 = result.GetInt32("Result");
            Assert.That(v2, Is.EqualTo(9));

            context.SetArgs("x", 13);
            context.SetArgs("y", 5);
            result = context.ExecuteFlow(flow).Result;
            v2 = result.GetInt32("Result");
            Assert.That(v2, Is.EqualTo(20));

        }
        private static FunctionFlowConfig CreateSwitchFlow()
        {
            var flow = new FunctionFlowConfig();
            var action1 = new FunctionFlowAction
            {
                ID = "plus",
                FunctionName = "Plus",
                Parameters = new CollectionBase<ActionInputParameter>
                {
                    new ActionInputParameter
                    {
                        Name = "x",
                        IsInherit=false,
                        Expression = "x"
                    },
                    new ActionInputParameter
                    {
                        Name = "y",
                        IsInherit=false,
                        Expression = "y"
                    },
                },
            };
            action1.ResultParameters.Add(new ActionOutputParameter
            {
                Name = "Value",
                ResetInputName = "v"
            });
            flow.Actions.Add(action1);


            var sitem1 = new SwitchActionItem
            {
                Condition = "v>20",
                Actions = new CollectionBase<FlowActionBase>
                {
                    new FunctionFlowAction
                    {
                        ID = "plus",
                        FunctionName = "Plus",
                        Parameters = new CollectionBase<ActionInputParameter>
                        {
                            new ActionInputParameter
                            {
                                Name = "x",
                                IsInherit=false,
                                Expression = "v"
                            },
                            new ActionInputParameter
                            {
                                Name = "y",
                                IsInherit=false,
                                Expression = "-1"
                            },
                        },
                        ResultParameters=new CollectionBase<ActionOutputParameter>
                        {
                            new ActionOutputParameter
                            {
                                Name = "Value",
                                ResultName = "v2"
                            }
                        }
                    }
                }
            };

            var sitem2 = new SwitchActionItem
            {
                Condition = "v<10",
                Actions = new CollectionBase<FlowActionBase>
                {
                    new FunctionFlowAction
                    {
                        ID = "plus",
                        FunctionName = "Plus",
                        Parameters = new CollectionBase<ActionInputParameter>
                        {
                            new ActionInputParameter
                            {
                                Name = "x",
                                IsInherit=false,
                                Expression = "v"
                            },
                            new ActionInputParameter
                            {
                                Name = "y",
                                IsInherit=false,
                                Expression = "1"
                            },
                        },
                        ResultParameters=new CollectionBase<ActionOutputParameter>
                        {
                            new ActionOutputParameter
                            {
                                Name = "Value",
                                ResultName = "v2"
                            }
                        }
                    }
                }
            };
            var sitem3 = new SwitchActionItem
            {
                Actions = new CollectionBase<FlowActionBase>
                {
                    new FunctionFlowAction
                    {
                        ID = "plus",
                        FunctionName = "Plus",
                        Parameters = new CollectionBase<ActionInputParameter>
                        {
                            new ActionInputParameter
                            {
                                Name = "x",
                                IsInherit=false,
                                Expression = "v"
                            },
                            new ActionInputParameter
                            {
                                Name = "y",
                                IsInherit=false,
                                Expression = "2"
                            },
                        },
                        ResultParameters=new CollectionBase<ActionOutputParameter>
                        {
                            new ActionOutputParameter
                            {
                                Name = "Value",
                                ResultName = "v2"
                            }
                        }
                    }
                }
            };
            var switchAction = new SwitchAction
            {
                Name = "switch",
                Parameters = new CollectionBase<ActionInputParameter>
                {
                    new ActionInputParameter
                    {
                        Name = "v",
                        IsInherit=false,
                        Expression = "v"
                    }
                },
                Items = new CollectionBase<SwitchActionItem>
                {
                    sitem1,sitem2
                },
                DefaultItem = sitem3,
                ResultParameters = new CollectionBase<ActionOutputParameter>
                {
                    new ActionOutputParameter
                    {
                        Name = "v2",
                        ResultName = "v2"
                    }
                }
            };

            flow.Actions.Add(switchAction);

            flow.Parameters.Add(new FlowInputParameter
            {
                Name = "x",
                DefaultValue = "=x"
            });
            flow.Parameters.Add(new FlowInputParameter
            {
                Name = "y",
                DefaultValue = "=y"
            });

            flow.Results.Add(new FlowResultParameter
            {
                Name = "Result",
                Expression = "v2"
            });
            return flow;
        }
        #endregion

        #region 测试循环逻辑
        [Test]
        public void ForEachLogic()
        {
            FunctionFlowConfig flow = CreateForEachFlow();
            var context = new FlowContext(new Dictionary<string, object>
            {
                { "items",
                    new List<object>{
                        new {x=12,y=13},
                        new {x=3,y=5},
                        new {x=13,y=5}
                    }
                }
            });

            var result = context.ExecuteFlow(flow).Result;
            var items = result.GetValue("Result");
            Assert.IsNotNull(items);
            Assert.IsTrue(items is List<DynamicEntity>);
            var list = (List<DynamicEntity>)items;
            if (list != null)
            {
                Assert.AreEqual(list.Count, 3);
                Assert.AreEqual(list[0].GetInt32("v2"), 24);
                Assert.AreEqual(list[1].GetInt32("v2"), 9);
                Assert.AreEqual(list[2].GetInt32("v2"), 20);
            }

        }

        private static FunctionFlowConfig CreateForEachFlow()
        {
            var flow = new FunctionFlowConfig();

            var feach = new ForEachAction
            {
                ItemsSource = "items",
                ItemName = "item",
                ItemIndexName = "i",
            };

            var action1 = new FunctionFlowAction
            {
                ID = "plus",
                FunctionName = "Plus",
                Parameters = new CollectionBase<ActionInputParameter>
                {
                    new ActionInputParameter
                    {
                        Name = "x",
                        IsInherit=false,
                        Expression = "item.x"
                    },
                    new ActionInputParameter
                    {
                        Name = "y",
                        IsInherit=false,
                        Expression = "item.y"
                    },
                },
            };
            action1.ResultParameters.Add(new ActionOutputParameter
            {
                Name = "Value",
                ResetInputName = "v"
            });
            feach.Actions.Add(action1);

            #region switch
            var sitem1 = new SwitchActionItem
            {
                Condition = "v>20",
                Actions = new CollectionBase<FlowActionBase>
                {
                    new FunctionFlowAction
                    {
                        ID = "plus",
                        FunctionName = "Plus",
                        Parameters = new CollectionBase<ActionInputParameter>
                        {
                            new ActionInputParameter
                            {
                                Name = "x",
                                IsInherit=false,
                                Expression = "v"
                            },
                            new ActionInputParameter
                            {
                                Name = "y",
                                IsInherit=false,
                                Expression = "-1"
                            },
                        },
                        ResultParameters=new CollectionBase<ActionOutputParameter>
                        {
                            new ActionOutputParameter
                            {
                                Name = "Value",
                                ResultName = "v2"
                            }
                        }
                    }
                }
            };

            var sitem2 = new SwitchActionItem
            {
                Condition = "v<10",
                Actions = new CollectionBase<FlowActionBase>
                {
                    new FunctionFlowAction
                    {
                        ID = "plus",
                        FunctionName = "Plus",
                        Parameters = new CollectionBase<ActionInputParameter>
                        {
                            new ActionInputParameter
                            {
                                Name = "x",
                                IsInherit=false,
                                Expression = "v"
                            },
                            new ActionInputParameter
                            {
                                Name = "y",
                                IsInherit=false,
                                Expression = "1"
                            },
                        },
                        ResultParameters=new CollectionBase<ActionOutputParameter>
                        {
                            new ActionOutputParameter
                            {
                                Name = "Value",
                                ResultName = "v2"
                            }
                        }
                    }
                }
            };
            var sitem3 = new SwitchActionItem
            {
                Actions = new CollectionBase<FlowActionBase>
                {
                    new FunctionFlowAction
                    {
                        ID = "plus",
                        FunctionName = "Plus",
                        Parameters = new CollectionBase<ActionInputParameter>
                        {
                            new ActionInputParameter
                            {
                                Name = "x",
                                IsInherit=false,
                                Expression = "v"
                            },
                            new ActionInputParameter
                            {
                                Name = "y",
                                IsInherit=false,
                                Expression = "2"
                            },
                        },
                        ResultParameters=new CollectionBase<ActionOutputParameter>
                        {
                            new ActionOutputParameter
                            {
                                Name = "Value",
                                ResultName = "v2"
                            }
                        }
                    }
                }
            };
            var switchAction = new SwitchAction
            {
                Name = "switch",
                Parameters = new CollectionBase<ActionInputParameter>
                {
                    new ActionInputParameter
                    {
                        Name = "v",
                        IsInherit=false,
                        Expression = "v"
                    }
                },
                Items = new CollectionBase<SwitchActionItem>
                {
                    sitem1,sitem2
                },
                DefaultItem = sitem3,
                ResultParameters = new CollectionBase<ActionOutputParameter>
                {
                    new ActionOutputParameter
                    {
                        Name = "v2",
                        ResultName = "v2"
                    }
                }
            };
            #endregion

            feach.Actions.Add(switchAction);
            feach.ResultParameters.Add(new ActionOutputParameter
            {
                Name = "ResultList",
                ResultName = "ResultList"
            });

            flow.Actions.Add(feach);

            flow.Parameters.Add(new FlowInputParameter
            {
                Name = "items"
            });

            flow.Results.Add(new FlowResultParameter
            {
                Name = "Result",
                Expression = "ResultList"
            });
            return flow;
        }
        #endregion

        #region 测试循环逻辑
        [Test]
        public void WhileLogic()
        {
            FunctionFlowConfig flow = CreateWhileFlow();
            var context = new FlowContext(new Dictionary<string, object>
            {
                { "items",
                    new List<DynamicEntity>{
                        new DynamicEntity("x",12,"y",13),
                        new DynamicEntity("x",3,"y",5),
                        new DynamicEntity("x",13,"y",5)
                    }
                }
            });

            var result = context.ExecuteFlow(flow).Result;
            var items = result.GetValue("Result");
            Assert.IsNotNull(items);
            Assert.IsTrue(items is List<DynamicEntity>);
            var list = (List<DynamicEntity>)items;
            
            if (list != null)
            {
                Assert.AreEqual(list.Count, 3);
                Assert.AreEqual(24, list[0].GetInt32("v2"));
                Assert.AreEqual(list[1].GetInt32("v2"), 9);
                Assert.AreEqual(list[2].GetInt32("v2"), 20);
            }
        }

        private static FunctionFlowConfig CreateWhileFlow()
        {
            var flow = new FunctionFlowConfig();

            var w = new WhileAction
            {
                Loop = "index<items.Count",
                IndexName = "index"
            };

            w.Parameters.Add(new ActionInputParameter
            {
                Name = "items",
                Expression="items"
            });

            var action1 = new FunctionFlowAction
            {
                ID = "plus",
                FunctionName = "Plus",
                Parameters = new CollectionBase<ActionInputParameter>
                {
                    new ActionInputParameter
                    {
                        Name = "x",
                        IsInherit=false,
                        Expression = "items[index].x"
                    },
                    new ActionInputParameter
                    {
                        Name = "y",
                        IsInherit=false,
                        Expression = "items[index].y"
                    },
                },
            };
            action1.ResultParameters.Add(new ActionOutputParameter
            {
                Name = "Value",
                ResetInputName = "v"
            });
            w.Actions.Add(action1);

            #region switch
            var sitem1 = new SwitchActionItem
            {
                Condition = "v>20",
                Actions = new CollectionBase<FlowActionBase>
                {
                    new FunctionFlowAction
                    {
                        ID = "plus",
                        FunctionName = "Plus",
                        Parameters = new CollectionBase<ActionInputParameter>
                        {
                            new ActionInputParameter
                            {
                                Name = "x",
                                IsInherit=false,
                                Expression = "v"
                            },
                            new ActionInputParameter
                            {
                                Name = "y",
                                IsInherit=false,
                                Expression = "-1"
                            },
                        },
                        ResultParameters=new CollectionBase<ActionOutputParameter>
                        {
                            new ActionOutputParameter
                            {
                                Name = "Value",
                                ResultName = "v2"
                            }
                        }
                    }
                }
            };

            var sitem2 = new SwitchActionItem
            {
                Condition = "v<10",
                Actions = new CollectionBase<FlowActionBase>
                {
                    new FunctionFlowAction
                    {
                        ID = "plus",
                        FunctionName = "Plus",
                        Parameters = new CollectionBase<ActionInputParameter>
                        {
                            new ActionInputParameter
                            {
                                Name = "x",
                                IsInherit=false,
                                Expression = "v"
                            },
                            new ActionInputParameter
                            {
                                Name = "y",
                                IsInherit=false,
                                Expression = "1"
                            },
                        },
                        ResultParameters=new CollectionBase<ActionOutputParameter>
                        {
                            new ActionOutputParameter
                            {
                                Name = "Value",
                                ResultName = "v2"
                            }
                        }
                    }
                }
            };
            var sitem3 = new SwitchActionItem
            {
                Actions = new CollectionBase<FlowActionBase>
                {
                    new FunctionFlowAction
                    {
                        ID = "plus",
                        FunctionName = "Plus",
                        Parameters = new CollectionBase<ActionInputParameter>
                        {
                            new ActionInputParameter
                            {
                                Name = "x",
                                IsInherit=false,
                                Expression = "v"
                            },
                            new ActionInputParameter
                            {
                                Name = "y",
                                IsInherit=false,
                                Expression = "2"
                            },
                        },
                        ResultParameters=new CollectionBase<ActionOutputParameter>
                        {
                            new ActionOutputParameter
                            {
                                Name = "Value",
                                ResultName = "v2"
                            }
                        }
                    }
                }
            };
            var switchAction = new SwitchAction
            {
                Name = "switch",
                Parameters = new CollectionBase<ActionInputParameter>
                {
                    new ActionInputParameter
                    {
                        Name = "v",
                        IsInherit=false,
                        Expression = "v"
                    }
                },
                Items = new CollectionBase<SwitchActionItem>
                {
                    sitem1,sitem2
                },
                DefaultItem = sitem3,
                ResultParameters = new CollectionBase<ActionOutputParameter>
                {
                    new ActionOutputParameter
                    {
                        Name = "v2",
                        ResultName = "v2"
                    }
                }
            };
            #endregion

            w.Actions.Add(switchAction);
            w.ResultParameters.Add(new ActionOutputParameter
            {
                Name = "ResultList",
                ResultName = "ResultList"
            });

            flow.Actions.Add(w);

            flow.Parameters.Add(new FlowInputParameter
            {
                Name = "items"
            });

            flow.Results.Add(new FlowResultParameter
            {
                Name = "Result",
                Expression = "ResultList"
            });
            return flow;
        }
        #endregion
    }

    public class FlowTestMethods
    {
        public static IFunctionResult Plus(int x, int y)
        {
            return new FunctionResult().SetData("Value", x + y);
        }
        public static IFunctionResult Plus1(int x)
        {
            return new FunctionResult().SetData("Value", x + 1);
        }
    }
}
