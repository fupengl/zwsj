var ajax = function (config) { // 封装、简化AJAX
    Ext.Ajax.request({
        url: config.url, // 请求地址
        params: config.params, // 请求参数
        method: 'post', // 方法
        callback: function (options, success, response) {
            config.callback(Ext.JSON.decode(response.responseText));
            // 调用回调函数
        }
    });
    return false;
};
var loadMenu = function () {
    Ext.getBody().mask('正在加载系统菜单....');
    ajax({
        url: "/TianYuanDataCenter/SysMenu", // 获取面板的地址
        params: {
            nodeType: "list"
        },
        callback: addTree
    });
};
function addTree(data) {
    Ext.getBody().unmask();
    for (var i = 0; i < data.length; i++) {
        App.tree.add(Ext.create("Ext.tree.Panel", {
            title: data[i].text,
            iconCls: data[i].iconCls,
            //useArrows: true,
            autoScroll: true,
            rootVisible: false,
            viewConfig: {
                loadingText: "正在加载..."
            },
            store: createStore(data[i].id),
            listeners: {
                afterlayout: function () {
                    if (this.getView().el) {
                        var el = this.getView().el;
                        var table = el
                                    .down("table.x-grid-table");
                        if (table) {
                            table.setWidth(el.getWidth());
                        }
                    }
                },
                itemclick: function (view, node) {
                    if (App.tab.items.containsKey("tab_" + node.id)) {
                        App.tab.setActiveTab(App.tab.getComponent("tab_" + node.id));
                    } else {
                        if (node.isLeaf()) { //判断是否是根节点
                            if (node.data.type === 'URL') { //判断资源类型
                                //                                        var panel = new Ext.ux.IFrame({
                                //                                            xtype: 'iframepanel',
                                //                                            id: "tab_" + node.id,
                                //                                            title: node.data.text,
                                //                                            //iconCls: node.data.text,
                                //                                            iconCls: 'icon-activity',
                                //                                            closable: true,
                                //                                            layout: 'fit',
                                //                                            loadMask: '页面加载中...',
                                //                                            border: false
                                //                                        });

                                var panel = Ext.create('Ext.panel.Panel', {
                                    id: "tab_" + node.id,
                                    title: node.data.text,
                                    closable: true,
                                    iconCls: 'icon-applicationdouble',
                                    //iconCls: 'icon-activity',
                                    html: '<iframe style="overflow:auto;width:100%; height:100%;" frameborder="0" src=\"' + node.data.url + '\" scrolling="auto"></iframe>'
                                });
                                App.tab.add(panel);
                                App.tab.setActiveTab(panel);
                                //panel.load(node.data.url);

                            } else if (node.data.type === 'COMPONENT') {
                                var panel = Ext.create(node.data.url, {
                                    id: "tab_" + node.id,
                                    title: node.data.text,
                                    closable: true,
                                    iconCls: 'icon-applicationdouble'
                                    //iconCls: 'icon-activity'
                                });
                                App.tab.add(panel);
                                App.tab.setActiveTab(panel);
                            }
                        }
                    }
                }
            }
        }));
        App.tree.doLayout();

    }
}

var model = Ext.define("TreeModel", { // 定义树节点数据模型
    extend: "Ext.data.Model",
    fields: [{ name: "id", type: "string" },
						{ name: "text", type: "string" },
						{ name: "iconCls", type: "string" },
						{ name: "leaf", type: "boolean" },
						{ name: 'type' },
						{ name: 'url'}]
});
var createStore = function (id) { // 创建树面板数据源
    var me = this;
    return Ext.create("Ext.data.TreeStore", {
        defaultRootId: id, // 默认的根节点id
        model: model,
        proxy: {
            type: "ajax", // 获取方式
            url: "/TianYuanDataCenter/SysMenu", // 获取树节点的地址
            extraParams: {
                action: "node"
            }
        },
        clearOnLoad: true,
        nodeParam: "id"// 设置传递给后台的参数名,值是树节点的id属性
    });
};