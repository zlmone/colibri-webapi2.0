import { Component, Input, OnInit, OnChanges, EventEmitter, Output } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ControTypes } from '../../../../shared/constants/control-types.constant';

import { QuestionTransferService } from '../../../../shared/transfers/question-transfer.service';
import { QuestionControlService } from '../../../../shared/services/question-control.service';
// import { AvailableQuestions } from '../../../../shared/models/form-builder/form-control/available-question.model';
// import { DropdownQuestion } from '../../../../shared/Models/form-builder/question-dropdown.model';
import { QuestionBase } from '../../../../shared/Models/form-builder/question-base.model';
// import { TextboxQuestion } from '../../../../shared/Models/form-builder/question-textbox.model';
// import { CheckboxQuestion } from '../../../../shared/Models/form-builder/question-checkbox.model';


@Component({
    selector: 'survey-from-builder',
    templateUrl: './survey-form-builder.component.html',
    styleUrls: ['./survey-form-builder.component.scss'],
    providers: [QuestionControlService]
})
export class SurveyFormBuilderComponent implements OnInit {
    @Input() questionSettings: any;
    @Input() questions: QuestionBase<any>[] = [];
    @Input() question: QuestionBase<any>;
    form: FormGroup;

    editQuestionKey = '';

    @Output()
    temporaryAnser: EventEmitter<any> = new EventEmitter<any>();
    payLoad = '';
    newquestion: any;

    constructor(
        private questionTransferService: QuestionTransferService,
        private qcs: QuestionControlService,
        public questionControlService: QuestionControlService,
        // private fb: FormBuilder
    ) {
        this.questionTransferService.getDropQuestion().subscribe((data: any) => {
            // remove question
            this.form.removeControl(data.id);
            this.sortQuestionByIndex();
        });
    }

    sortQuestionByIndex() {
        this.questions.forEach(x => {
            const indexOf = this.questions.indexOf(x);
            x.order = indexOf;
        });
    }

    setQuestionOption(question: any, checked: boolean) {
        if (checked) {
            this.questionTransferService.setQuestionOption(
                {
                    question: question,
                    control: this.form.controls[question.id]
                }
            );

        }


    }

    editQuestionEvent(item: any) {

    }

    ngOnInit() {
        this.form = this.qcs.toFormGroup(this.questions);

    }




    changeQuestionOrders(item: any, index: number) {
        this.questions.forEach(x => {
            const indexOf = this.questions.indexOf(x);
            x.order = indexOf;
        });
    }


    addNewQuestion($event: any, index: number) {
        // organisere question orden
        this.questions.forEach(x => {
            const indexOf = this.questions.indexOf(x);
            x.order = indexOf;
        });

        this.questions.splice(index, 1); // remove AvailableQuestions object
        switch ($event.dragData.type) {
            case ControTypes.textbox: {
                this.newquestion = this.questionControlService.addTextboxControl(index);
                break;
            }
            case ControTypes.textarea: {
                this.newquestion = this.questionControlService.addTextareaControl(index);
                break;
            }
            case ControTypes.radio: {
                this.newquestion = this.questionControlService.addRadioButtonControl(index);
                break;
            }
            case ControTypes.checkbox: {
                this.newquestion = this.questionControlService.addCheckBoxControl(index);
                break;
            }
            case ControTypes.dropdown: {
                this.newquestion = this.questionControlService.addDropdownControl(index);
                break;
            }
            case ControTypes.gridRadio: {
                this.newquestion = this.questionControlService.addGridRadioControl(index);
                break;
            }

            default: {
                console.log('Invalid choice');
                break;
            }
        }
        const group: any = {};
        this.form.addControl(this.newquestion.id, this.questionControlService.addTypeAnswer(this.newquestion, group)
        );

        this.questions.push(this.newquestion);
        this.questions.sort((a, b) => a.order - b.order);
    }


    onSubmit() {
        this.payLoad = this.form.value;
        this.temporaryAnser.emit(this.payLoad);
    }
}
