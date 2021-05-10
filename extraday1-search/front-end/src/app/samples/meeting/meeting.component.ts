import { Component, OnInit, ViewChild } from '@angular/core';
import { NzDatePickerComponent } from 'ng-zorro-antd/date-picker';
import { CommonService } from 'src/app/service/common.service';

@Component({
  selector: 'app-meeting',
  templateUrl: './meeting.component.html',
  styleUrls: ['./meeting.component.scss'],
})
export class MeetingComponent implements OnInit {
  SearchEvent1 = true;

  // Control
  showMeetingParticipates;

  showCode = false;

  peopleLoading = false;

  mockEventdata1: any[] = [];

  mockMeetingDetails: any = {};

  constructor(private commonService: CommonService) {}

  ngOnInit(): void {}

  isSpinning = false;
  searchTerms: string = '';
  showConfiguration = false;

  public getMonthAndDay(date: string): string {
    return date.substr(0, 10);
  }

  public getHourAndMinutes(date: string): string {
    return date.substring(11, 16);
  }

  meetingOpen(data: any): void {
    this.isSpinning = true;
    this.commonService.getMeetingDetails(data.hitId).subscribe(
      (data) => {
        this.isSpinning = false;
        this.showMeetingParticipates = true;
        this.mockMeetingDetails = data;
      },
      (error) => {
        this.isSpinning = false;
        alert(error['message']);
      }
    );
  }

  meetingClose(): void {
    this.showMeetingParticipates = false;
  }

  executeSearchEvent(): void {
    if (this.searchTerms == '') {
      alert('Search term cannot be empty');
      return;
    }
    this.isSpinning = true;
    this.commonService
      .SearchMeeting(this.searchTerms, this.startValue, this.endValue)
      .subscribe(
        (data) => {
          this.mockEventdata1 = data['value'][0]['hitsContainers'][0]['hits'];
          this.isSpinning = false;
        },
        (error) => {
          this.isSpinning = false;
          alert(error['message']);
        }
      );
  }

  configOpen(): void {
    this.showConfiguration = true;
  }

  configClose(): void {
    this.showConfiguration = false;
  }

  startValue: Date | null = null;
  endValue: Date | null = null;
  @ViewChild('endDatePicker') endDatePicker!: NzDatePickerComponent;

  disabledStartDate = (startValue: Date): boolean => {
    if (!startValue || !this.endValue) {
      return false;
    }
    return startValue.getTime() > this.endValue.getTime();
  };

  disabledEndDate = (endValue: Date): boolean => {
    if (!endValue || !this.startValue) {
      return false;
    }
    return endValue.getTime() <= this.startValue.getTime();
  };

  handleStartOpenChange(open: boolean): void {
    if (!open) {
      this.endDatePicker.open();
    }
    console.log('handleStartOpenChange', open);
  }

  handleEndOpenChange(open: boolean): void {
    console.log('handleEndOpenChange', open);
  }

  showDateRange() {
    console.log(this.startValue, this.endValue);
  }

  codeOpen(): void {
    this.showCode = true;
  }

  codeClose(): void {
    this.showCode = false;
  }
}
