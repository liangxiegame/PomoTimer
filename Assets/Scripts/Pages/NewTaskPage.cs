using System.Collections.Generic;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.service;
using Unity.UIWidgets.widgets;

namespace PomodoroApp
{
    public class NewTaskPage : StatefulWidget
    {
        public override State createState()
        {
            return new NewTaskState();
        }
    }

    class NewTaskState : State<NewTaskPage>
    {
        private TextEditingController mTitleController, mDescriptionController;

        private const int  MAX_TITLE_LENGTH   = 24;
        private       bool mSaveButtonVisible = false;

        void SaveTaskAndClose()
        {
            var title = mTitleController.text;
            var description = mDescriptionController.text;

            if (title.Trim().isEmpty())
            {
                return;
            }

            Navigator.pop(context, new Task(null, title, description, false));
        }

        public override void initState()
        {
            base.initState();
            mDescriptionController = new TextEditingController(text:"");
            mTitleController = new TextEditingController(text:"");
        }

        public override void dispose()
        {
            mTitleController.dispose();
            mDescriptionController.dispose();
            base.dispose();
        }

        public override Widget build(BuildContext context)
        {
            Widget saveButton = new IconButton(
                onPressed: SaveTaskAndClose,
                tooltip: "Save task",
                icon: new Icon(
                    Icons.save,
                    size: 32,
                    color: Theme.of(context).primaryColor
                )
            );

            return new Scaffold(
                body: new Material(
                    color: Colors.white,
                    child: new Stack(
                        children: new List<Widget>()
                        {
                            new ListView(
                                shrinkWrap: false,
                                children: new List<Widget>()
                                {
                                    new Padding(
                                        padding: EdgeInsets.only(top: 8, bottom: 8),
                                        child: new Row(
                                            children: new List<Widget>()
                                            {
                                                new IconButton(
                                                    onPressed: () => { Navigator.of(context).pop(); },
                                                    icon: new Icon(
                                                        Icons.arrow_back,
                                                        size: 32,
                                                        color: Colors.grey
                                                    )
                                                ),
                                                new Spacer(),
                                                mSaveButtonVisible ? saveButton : new Spacer()
                                            }
                                        )
                                    ),
                                    new TextField(
                                        maxLength: MAX_TITLE_LENGTH,
                                        controller: mTitleController,
                                        style: new TextStyle(
                                            fontSize: 24.0f,
                                            color: Colors.black
                                        ),
                                        decoration: new InputDecoration(
                                            hintText: "Task title",
                                            filled: true,
                                            fillColor: Colors.white,
                                            border: InputBorder.none,
                                            contentPadding:
                                            EdgeInsets.only(left: 32, right: 32, bottom: 4)
                                        ),
                                        onChanged: (text) =>
                                        {
                                            var needSaveButton = mTitleController.text.Trim().Length > 0;

                                            if (mSaveButtonVisible != needSaveButton)
                                            {
                                                setState(() => { mSaveButtonVisible = needSaveButton; });
                                            }
                                        }
                                    ),
                                    new TextField(
                                        controller: mDescriptionController,
                                        keyboardType: TextInputType.multiline,
                                        maxLines: 10,
                                        style: new TextStyle(
                                            fontSize: 18.0f,
                                            color: Colors.black
                                        ),
                                        decoration: new InputDecoration(
                                            hintText: "Description",
                                            filled: true,
                                            fillColor: Colors.white,
                                            border: InputBorder.none,
                                            counterText: "",
                                            contentPadding:
                                            EdgeInsets.only(left: 32, right: 32, bottom: 4)
                                        )
                                    )
                                })
                        }
                    )
                )
            );
        }
    }
}