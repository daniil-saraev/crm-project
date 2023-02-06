import { Component, OnInit } from '@angular/core';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
})
export class NavbarComponent implements OnInit {
  private _items: MenuItem[];

  ngOnInit() {
    this._items = [
      { label: 'Главня', url: '/' },
      { label: 'Услуги', url: '/services' },
      { label: 'Проекты', url: '/projects' },
      { label: 'Контакты', url: '/contacts' },
    ];
  }

  public get items(): MenuItem[] {
    return this._items;
  }

  constructor() {
    this._items = new Array<MenuItem>();
  }
}
