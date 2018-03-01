# HPMessageCenter
高性能消息分发中心。用户只需写好restful接口，在portal里面配置消息的处理地址，消息消费者就会自动访问相关接口，完成消息任务。

### 部署说明

 **配置数据库连接字符串** 

打开MessageCenter.Portal\Configuration\Data\Database.config
![输入图片说明](https://gitee.com/uploads/images/2018/0301/161547_191f3532_13001.png "TIM图片20180301161514.png")

在图片中画红线的地方修改链接字符串

 **配置RabbitMQ连接属性** 

打开MessageCenter.Portal\appsettings.json
![输入图片说明](https://gitee.com/uploads/images/2018/0301/161838_940d3b45_13001.png "TIM截图20180301161815.png")

在图片中画红线的地方修改RabbitMQ地址和用户名密码

部署MessageCenter.Portal到IIS或者Docker中，即可访问

### 使用说明

 **在首页中配置App和Exchange** 
![输入图片说明](https://gitee.com/uploads/images/2018/0301/162227_02b643e3_13001.png "TIM截图20180301162157.png")

 **在Topic管理页面中配置Topic信息** 
![输入图片说明](https://gitee.com/uploads/images/2018/0301/162325_f9047cb3_13001.png "TIM截图20180301162251.png")

ProcessorConfig为消息消费者处理消息的Restful接口

 **最后在消费者管理页面中添加Sever要运行的Consumer** 
![输入图片说明](https://gitee.com/uploads/images/2018/0301/162558_5e4275d8_13001.png "TIM图片20180301162542.png")

此时，通过Publisher/Publish接口发送消息到RabbitMQ，系统中的消费者会自动访问配置的对应消息处理接口处理消息。

### 为什么要开发这样一个消息系统

使用此消息系统处理消息，开发者只需调用接口发送消息、写消息处理的接口，不必关心MQ的实现和使用，使开发者更关注业务，提高开发效率。

### 扩展性

MessageTransit模块是一个高度抽象的模块，开发者可以继承它的接口实现其他MQ（ActiveMQ和RocketMQ等）的对接。

### 性能

一个Sever里每个Topic对应一个消费者。一个消费者一次处理一个消息，如果发生消息处理不及时的情况，可以部署多个Sever，并在消费者管理页面添加对应的消息消费者。如果消息数量进一步提高，就需要同步增加消息Restful处理接口的处理能力。比如，一个消息处理接口可以处理每分钟300个并发，一个Sever每分钟只能处理60个消息，此时可以部署5个Sever。消息进一步增加，消息处理接口性能达到瓶颈，增加消息处理接口的处理能力，再增加Sever数即可提高性能。