import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink} from '@chakra-ui/react'

export default function Navbar() {
  return (
    <div>
      <Breadcrumb fontFamily="inherit" fontWeight="600" justifyContent="center" display="flex">
          <BreadcrumbItem>
            <BreadcrumbLink href='/'>Главная</BreadcrumbLink>
          </BreadcrumbItem>
          <BreadcrumbItem>
            <BreadcrumbLink href='/services'>Услуги</BreadcrumbLink>
          </BreadcrumbItem>
          <BreadcrumbItem>
            <BreadcrumbLink href='/projects'>Проекты</BreadcrumbLink>
          </BreadcrumbItem>
          <BreadcrumbItem>
            <BreadcrumbLink href='/contacts'>Контакты</BreadcrumbLink>
          </BreadcrumbItem>
      </Breadcrumb>
    </div>
  );
}