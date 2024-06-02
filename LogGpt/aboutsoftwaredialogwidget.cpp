#include "aboutsoftwaredialogwidget.h"
#include "ui_aboutsoftwaredialogwidget.h"

AboutSoftwareDialogWidget::AboutSoftwareDialogWidget(QWidget *parent) :
    QDialog(parent),
    ui(new Ui::AboutSoftwareDialogWidget)
{
    ui->setupUi(this);
    init();
}

AboutSoftwareDialogWidget::~AboutSoftwareDialogWidget()
{
    delete ui;
}

void AboutSoftwareDialogWidget::init()
{
    this->resize(_Width,_Height);
    this->setWindowTitle("关于");
    this->setWindowIcon(QIcon(":/res/u77.svg"));
    setWindowFlags(windowFlags() & ~Qt::WindowContextHelpButtonHint);

    QString information_str="LogChat客户端 Version-7.2\n"
                            "By BiliBili: LearningLog\n"
                            "GitHub:https://github.com/log159/LogChat\n"
                            "#before--------------------------\n"
                            "支持ChatGPT、讯飞星火大语言模型\n"
                            "支持vits、bert-vits2、w2v2-vits\n"
                            "#6.0-----------------------------\n"
                            "支持基本Live2D\n"
                            "#7.0-----------------------------\n"
                            "支持Live2D基本自定义\n"
                            "#7.1\n"
                            "Live2d窗口显示不适配问题\n"
                            "Live2d窗口遮挡Web界面问题\n"
                            "#7.2\n"
                            "发送消息框光标问题\n"
                            "UI优化，感谢GitHub@Zao-chen\n"
                            "使得设置信息可以保存\n"
                            "控件初始信息同步优化\n"
                            "百度翻译处理不完全问题\n"
                            "有Bug欢迎反馈！\n";
    QFont font;
    font.setPixelSize(20);
    ui->textEdit_about->setFont(font);
    ui->textEdit_about->setReadOnly(true);
    ui->textEdit_about->setText(information_str);

    /*读取qss文件*/
    QFile file(":/main.qss");
    if(file.open(QFile::ReadOnly)){
        QString styleSheet = QLatin1String(file.readAll());
        this->setStyleSheet(styleSheet);
        file.close();
    }

}
