# aicodo
欢迎使用AiCodo，这是三个词：ai（爱）、coding、do，特别强调第一个不是A...I，而是“爱”，co代表coding，do代表行动，喜欢就去做

“AiCodo”是我将在c#开发中经常用到的基本类、函数（扩展函数）等集中在一个项目中，
逐步形成了一个类库，也逐渐形成一个开发的“习惯”。

由于工作的关系，先后加入了多个团队，有些是从零开始组建，觉得在团队初期需要这样
的一个框架，首先“她”能将一些基础工作完成（框架），然后再结合一些约定、规范，生
成相应的代码（代码工具），这样在开发中会大大提高开发效率。

我的想法中这不是一个框架，我希望这是一个约定，然后我们根据这个约定做自己的工具。
我把她共享出来的这一刻，她就不是私有的，您可以使用代码商用、发布。可以修改代码，
但请备注修改的部分，每个代码文件请保留原有的版权申明（有些文件没有，后面会加上），
加了申明不是限制大家的使用，你可以使用到任何项目，但请保留引用申明，以避免代码
的整体或部分的分享不会受到限制。

这是第一版的计划
* AiCodo					//基类、接口、帮助函数
* AiCodo.Data				//数据库操作及相关基础
* AiCodo.DBProviders		//具体某个数据库（版本）接口封装，包括表结构获取及其它公用操作相关，将来可能会每个数据库对应一个库，比如AiCodo.DBProviders.MySql
* AiCodo.Codes			//代码生成及模板引擎
* AiCodo.Web				//开发平台入口
* AiCodo.Tests            //测试WebApi的“测试项目”，目前只写了登录及增删改查

这是第一步，还会更多、更好
btw：有人帮我翻译吗？对，免费的那种(*￣︶￣)
