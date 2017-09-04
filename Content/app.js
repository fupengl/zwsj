Ext.Loader.setConfig({
	enabled : true
});

Ext.Loader.setPath({
    'Ext.ux': '/Content/ext4/ux',
    'Ext.app': '/Content/ext4/app'
});

Ext.require(['Ext.app.Portlet', 'Ext.app.PortalColumn', 'Ext.app.PortalPanel',
		'Ext.app.PortalDropZone', 'Ext.ux.TabReorderer',
		'Ext.ux.TabCloseMenu', 'Ext.ux.statusbar.StatusBar', 'Ext.ux.IFrame']);

var ajax = function(config) { // 封装、简化AJAX
	Ext.Ajax.request({
				url : config.url, // 请求地址
				params : config.params, // 请求参数
				method : 'post', // 方法
				callback : function(options, success, response) {
					config.callback(Ext.JSON.decode(response.responseText));
					// 调用回调函数
				}
			});
	return false;
};
Ext.onReady(function () {
    var tab = Ext.create('Ext.tab.Panel', {
        activeTab: 0,
        enableTabScroll: true,
        animScroll: true,
        border: true,
        autoScroll: true,
        region: 'center',
        split: true,
        items: [{
            iconCls: 'icon-activity',
            title: '平台首页',
            xtype: 'portalpanel',
            layout: 'column',
            items: [{
                xtype: 'portalcolumn',
                columnWidth: 0.7,
                items: [{ title: '新闻动态', height: 150, iconCls: 'icon-news' },
					                	{ title: '最新通知', height: 150, iconCls: 'icon-notice' },
					                	{ title: '业绩报表', height: 150, iconCls: 'icon-chart'}]
            }, {
                xtype: 'portalcolumn',
                columnWidth: 0.3,
                items: [{ title: '功能链接', height: 150, iconCls: 'icon-link' },
					                	{ title: '待办事项', height: 150, iconCls: 'icon-note' },
					                	{ title: '邮件列表', height: 150, iconCls: 'icon-email-list'}]
            }]
        }],
        plugins: [Ext.create('Ext.ux.TabReorderer'),
		        		  Ext.create('Ext.ux.TabCloseMenu', {
		        		      closeTabText: '关闭面板',
		        		      closeOthersTabsText: '关闭其他',
		        		      closeAllTabsText: '关闭所有'
		        		  })]
    });
    var tree = Ext.create("Ext.panel.Panel", {
        region: 'west',
        title: "系统菜单",
        width: 250,
        iconCls: "icon-tree",
        autoScroll: false,
        layout: 'accordion',
        collapsible: true,
        layoutConfig: {
            animate: true
        },
        split: true
    });
    var title = Ext.create("Ext.panel.Panel", {
        height: 80,
        html: '业务基础平台',
        region: 'north',
        split: true,
        bbar: [{
            iconCls: 'icon-user',
            text: '管理员'
        }, '-', {
            text: Ext.Date.format(new Date(), 'Y年m月d日')
        }, '->', {
            text: '退出',
            iconCls: 'icon-logout'
        }],
        bodyStyle: 'backgroud-color:#99bbe8;line-height : 50px;padding-left:20px;font-size:22px;color:#000000;font-family:黑体;font-weight:bolder;' +
								'background: -webkit-gradient(linear, left top, left bottom, color-stop(0%, rgba(153,187, 232, 0.4) ), color-stop(50%, rgba(153, 187, 232, 1) ),color-stop(0%, rgba(153, 187, 232, 0.4) ) )'
    });
    var footBar = new Ext.ux.StatusBar({
        region: "south",
        statusAlign:"left",
        items: [ "Test"]
    });
    Ext.create('Ext.container.Viewport', {
        layout: 'border',
        items: [title, tab, tree, footBar],
        listeners: {
            afterrender: function () {
                Ext.getBody().mask('正在加载系统菜单....');
                ajax({
                    url: "SysMenu", // 获取面板的地址
                    params: {
                        action: "list"
                    },
                    callback: addTree
                });
            }
        }
    });

    function addTree(data) {
        Ext.getBody().unmask();
        for (var i = 0; i < data.length; i++) {
            tree.add(Ext.create("Ext.tree.Panel", {
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
                        if (tab.items.containsKey("tab_" + node.id)) {
                            tab.setActiveTab(tab.getComponent("tab_" + node.id));
                        } else {
                            if (node.isLeaf()) { //判断是否是根节点
                                if (node.data.type === 'URL') { //判断资源类型
                                    panel = new Ext.ux.IFrame({
                                        xtype: 'iframepanel',
                                        id: "tab_" + node.id,
                                        title: node.data.text,
                                        //iconCls: node.data.text,
                                        iconCls: 'icon-activity',
                                        closable: true,
                                        layout: 'fit',
                                        loadMask: '页面加载中...',
                                        border: false
                                    });
                                    
//                                    var panel = Ext.create('Ext.panel.Panel', {
//                                        id: "tab_" + node.id,
//                                        title: node.data.text,
//                                        closable: true,
//                                        iconCls: 'icon-activity',
//                                        html: '<iframe style="overflow:auto;width:100%; height:100%;" frameborder="0" src=\"' + node.data.component + '\" scrolling="auto"></iframe>'
//                                    });
                                    tab.add(panel);
                                    tab.setActiveTab(panel);
                                    panel.load(node.data.component);
                                    
                                } else if (node.data.type === 'COMPONENT') {
                                    var panel = Ext.create(node.data.component, {
                                        id: "tab_" + node.id,
                                        title: node.data.text,
                                        closable: true,
                                        iconCls: 'icon-activity'
                                    });
                                    tab.add(panel);
                                    tab.setActiveTab(panel);
                                }
                            }
                        }
                    }
                }
            }));
            tree.doLayout();

        }
    }
    var model = Ext.define("TreeModel", { // 定义树节点数据模型
        extend: "Ext.data.Model",
        fields: [{ name: "id", type: "string" },
						{ name: "text", type: "string" },
						{ name: "iconCls", type: "string" },
						{ name: "leaf", type: "boolean" },
						{ name: 'type' },
						{ name: 'component'}]
    });
    var createStore = function (id) { // 创建树面板数据源
        var me = this;
        return Ext.create("Ext.data.TreeStore", {
            defaultRootId: id, // 默认的根节点id
            model: model,
            proxy: {
                type: "ajax", // 获取方式
                url: "SysMenu", // 获取树节点的地址
                extraParams: {
                    action: "node"
                }
            },
            clearOnLoad: true,
            nodeParam: "id"// 设置传递给后台的参数名,值是树节点的id属性
        });
    };
});