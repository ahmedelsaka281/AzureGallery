import { AlertifyService } from './../_services/alertify.service';
import { AzureService } from './../_services/azure.service';
import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { HttpEventType } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styles: []
})
export class HomeComponent implements OnInit {

  images: string[] = [];

  removeDisabled = false;
  downloadDisabled = false;
  uploadDisabled = false;


  constructor(private azureService: AzureService, private alertifyService: AlertifyService) { }

  ngOnInit() {
    this.getAllFiles();
  }

  getAllFiles() {
    this.azureService.getAllFiles().subscribe((res) => {
      this.images = res;
    });
  }

  deleteFile(img) {
    this.alertifyService.confirm('are you sure ?', () => {
      this.removeDisabled = true;
      const name = img.split('/')[4];
      this.azureService.deleteFile(name).subscribe((res) => {
        this.alertifyService.success(res);
        this.getAllFiles();
        this.removeDisabled = false;
      }, (err) => {
        this.alertifyService.error(err.error);
        this.removeDisabled = false;
      });
    });
  }

  downloadFile(img) {
    this.downloadDisabled = true;
    const name = img.split('/')[4];
    this.azureService.downloadFile(name).subscribe((res) => {
      this.alertifyService.success(res);
      this.downloadDisabled = false;
    }, (err) => {
      this.alertifyService.error(err.error);
      this.downloadDisabled = false;
    });
  }

  uploadFile(blop) {
    const file = blop.files;
    if (!file || file.length === 0) {
      this.alertifyService.warning('Choose file!');
      return;
    }

    this.uploadDisabled = true;
    const formData = new FormData();
    for (const img of file) {
      formData.append(img.name, img);
    }
    this.azureService.uploadFile(formData).subscribe(event => {
      if (event.type === HttpEventType.Response) {
        this.uploadDisabled = false;
        blop.value = null;
        this.getAllFiles();
      }
    });
  }

  deleteAllFiles() {
    this.alertifyService.confirm('are you sure ?', () => {

      this.azureService.deleteAllFiles().subscribe((res) => {
        this.alertifyService.success('done');
        this.getAllFiles();
      }, (err) => {
        this.alertifyService.error(err.statusText);
      });
    });
  }

}
