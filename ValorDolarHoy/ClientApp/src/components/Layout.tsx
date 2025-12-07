import React, { ReactNode } from 'react';
import { Container } from 'reactstrap';
import NavMenu from './nav-menu/NavMenu';

interface LayoutProps {
    children?: ReactNode;
}

export default function Layout({ children }: LayoutProps) {
    return (
        <>
            <NavMenu />
            <Container>
                {children}
            </Container>
        </>
    );
}

