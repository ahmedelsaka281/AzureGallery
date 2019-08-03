import { AuthService } from './../_services/auth.service';
import { AlertifyService } from './../_services/alertify.service';
import { Component, OnInit } from '@angular/core';
import { UserLoginDTO } from '../_models/UserLoginDTO';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: []
})
export class NavComponent implements OnInit {
  userLoginDTO: UserLoginDTO = new UserLoginDTO();


  constructor(private auth: AuthService, private alertify: AlertifyService) {

  }

  ngOnInit() { }

  login() {
    this.auth.login(this.userLoginDTO).subscribe(res => {
      this.alertify.success('logged in successfully!');
    }, err => {
      this.alertify.error(err.error);
    });
  }

}
