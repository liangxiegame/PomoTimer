using System.Collections.Generic;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.service;
using Unity.UIWidgets.widgets;
using Material = Unity.UIWidgets.material.Material;

namespace PomoTimerApp
{
    public class NewTaskPage : StatefulWidget
    {
        public override State createState()
        {
            return new NewTaskPageState();
        }
    }

    class NewTaskPageState : State<NewTaskPage>
    {

        private const int MAX_TEXT_LENGTH = 24;

        private bool mSaveButtonVisible = false;


        private TextEditingController mTitleController;
        private TextEditingController mDescriptionController;


        public override void initState()
        {
            base.initState();

            mTitleController = new TextEditingController();
            mDescriptionController = new TextEditingController();
        }


        void SaveAndClose()
        {
            var text = mTitleController.text;
            var description = mDescriptionController.text;

            if (text.Trim().isEmpty())
            {
                return;
            }

            Navigator.pop(context, new Task
            {
                Title = text,
                Description = description
            });
        }

        public override Widget build(BuildContext context)
        {
            var saveButton = new IconButton(
                icon: new Icon(
                    Icons.save,
                    size: 32,
                    color: Theme.of(context).primaryColor
                ),
                onPressed: SaveAndClose
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
                                        padding: EdgeInsets.symmetric(vertical: 8),
                                        child: new Row(
                                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                            children: new List<Widget>()
                                            {
                                                new IconButton(
                                                    icon: new Icon(
                                                        Icons.arrow_back,
                                                        size: 32,
                                                        color: Colors.grey),

                                                    onPressed: () => { Navigator.pop(context); }
                                                ),
                                                mSaveButtonVisible ? saveButton as Widget : new Container()
                                            }
                                        )
                                    ),
                                    new TextField(
                                        maxLength: MAX_TEXT_LENGTH,
                                        controller: mTitleController,
                                        style: new TextStyle(
                                            color: Colors.black,
                                            fontSize: 24.0f
                                        ),
                                        decoration: new InputDecoration(
                                            hintText: "Task title",
                                            filled: true,
                                            fillColor: Colors.white,
                                            contentPadding: EdgeInsets.only(left: 32, right: 32, bottom: 4),
                                            border: InputBorder.none
                                        ),
                                        onChanged: (text =>
                                        {
                                            var needShowSaveButton = text.Trim().Length > 0;

                                            if (needShowSaveButton != mSaveButtonVisible)
                                            {
                                                this.setState(() => { mSaveButtonVisible = needShowSaveButton; });
                                            }

                                        })
                                    ),
                                    new TextField(
                                        controller: mDescriptionController,
                                        keyboardType: TextInputType.multiline,
                                        maxLines: 10,
                                        style: new TextStyle(
                                            color: Colors.black,
                                            fontSize: 18
                                        ),
                                        decoration: new InputDecoration(
                                            hintText: "Description",
                                            filled: true,
                                            counterText: "",
                                            fillColor: Colors.white,
                                            contentPadding: EdgeInsets.only(left: 32, right: 32, bottom: 4),
                                            border: InputBorder.none
                                        )
                                    ),
                                }
                            )
                        }
                    )
                ));
        }
    }
}