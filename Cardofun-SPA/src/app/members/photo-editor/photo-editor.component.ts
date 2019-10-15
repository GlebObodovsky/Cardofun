import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Photo } from 'src/app/_models/photo';
import { FileUploader } from 'ng2-file-upload';
import { AuthService } from 'src/app/_services/auth/auth.service';
import { environment } from 'src/environments/environment';
import { UserService } from 'src/app/_services/user/user.service';
import { AlertifyService } from 'src/app/_services/alertify/alertify.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos: Photo[];
  @Output() getMemberPhotoChange = new EventEmitter<string>();
  constructor(private authService: AuthService, private userService: UserService,
    private alertifyService: AlertifyService) { }
  baseUrl = environment.apiUrl;
  uploader: FileUploader;
  currentMainPhoto: Photo;
  hasBaseDropZoneOver = false;

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  ngOnInit() {
    this.uploaderInit();
  }

  setMainPhoto(photo: Photo) {
    this.userService.setMainPhoto(this.authService.decodedToken.nameid, photo.id).subscribe(() => {
      this.currentMainPhoto = this.photos.filter(p => p.isMain === true)[0];
      this.currentMainPhoto.isMain = false;
      photo.isMain = true;
      this.getMemberPhotoChange.emit(photo.url);
    }, error => {
      this.alertifyService.error(error);
    });
  }

  uploaderInit() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });

    this.uploader.onAfterAddingFile = (file) => {file.withCredentials = false; };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response);
        // const photo = {
        //   id: res.id,
        //   url: res.url,
        //   dateAdded: res.dateAdded,
        //   description: res.description,
        //   isMain: res.isMain
        // };
        this.photos.push(res);
      }
    };
  }
}
